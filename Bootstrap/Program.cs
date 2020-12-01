using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using TestMyCode.CSharp.Core.Compiler;
using TestMyCode.CSharp.Core.Data;
using TestMyCode.CSharp.Core.Test;

namespace TestMyCode.CSharp.Bootstrap
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (!Program.FindMsBuild())
            {
                return;
            }

            Program.SetMsBuildDepsResolver();

            RootCommand rootCommand = Program.GenerateCommands();
            rootCommand.Handler = CommandHandler.Create(async (bool generatePointsFile, bool runTests, DirectoryInfo? projectDir, FileInfo? outputFile) =>
            {
                if (!generatePointsFile && !runTests)
                {
                    Console.WriteLine("You didn't give me a task! Use --help for... help! Leave a like if you found out this to be useful!");

                    return;
                }

                string projectDirectory = projectDir?.FullName ?? Environment.CurrentDirectory;

                ProjectCompiler compiler = new ProjectCompiler();

                ICollection<string> assemblyPaths;

                try
                {
                    assemblyPaths = compiler.CompileTestProjects(projectDirectory);
                }
                catch (CompilationFaultedException exception)
                {
                    Console.WriteLine(exception.Message);

                    Environment.Exit(1);

                    return;
                }

                TestProjectData projectData = new TestProjectData();
                foreach (string assemblyPath in assemblyPaths)
                {
                    projectData.LoadProject(assemblyPath);
                }

                if (generatePointsFile)
                {
                    FileInfo resultsFile = outputFile ?? new FileInfo(Path.Combine(projectDirectory, ".tmc_available_points.json"));

                    await WriteToFile(resultsFile, projectData.Points);
                }

                if (runTests)
                {
                    ProjectTestRunner testRunner = new ProjectTestRunner(projectData);
                    testRunner.RunAssemblies(projectData.Assemblies);

                    FileInfo resultsFile = outputFile ?? new FileInfo(Path.Combine(projectDirectory, ".tmc_test_results.json"));

                    await WriteToFile(resultsFile, testRunner.TestResults);
                }

                static async Task WriteToFile<T>(FileInfo resultsFile, T data)
                {
                    if (resultsFile.Exists)
                    {
                        resultsFile.Delete();
                    }

                    await using FileStream stream = resultsFile.Open(FileMode.OpenOrCreate, FileAccess.Write);

                    await JsonSerializer.SerializeAsync(stream, data, options: new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    });
                }
            });

            await rootCommand.InvokeAsync(args);
        }

        private static RootCommand GenerateCommands()
        {
            return new RootCommand(description: "TMC helper for running C# projects.")
            {
                new Option<bool>(aliases: new[]
                {
                    "--generate-points-file",
                    "-p",
                }, description: "Generates JSON file containing all the points that can be achieved from given project."),

                new Option<bool>(aliases: new[]
                {
                    "--run-tests",
                    "-t"
                }, description: "Runs tests for the project, by default using the working directory."),

                new Option<DirectoryInfo>(aliases: new[]
                {
                    "--project-dir"
                }, description: "Sets the directory where the project that should be used is located at.")
                {
                    Argument = new Argument<DirectoryInfo>().ExistingOnly()
                },

                new Option<FileInfo>(aliases: new[]
                {
                    "--output-file",
                    "-o"
                }, description: "The output file used to write results.")
            };
        }

        private static bool FindMsBuild()
        {
            string? msbuildPath = Environment.GetEnvironmentVariable("MSBUILD_EXE_PATH");
            if (msbuildPath is not null)
            {
                return true;
            }

            VisualStudioInstance? vsInstance = MSBuildLocator.QueryVisualStudioInstances(VisualStudioInstanceQueryOptions.Default)
                                                             .FirstOrDefault(i => i.Version.Major == Environment.Version.Major && i.Version.Minor == Environment.Version.Minor);
            if (vsInstance is not null)
            {
                MSBuildLocator.RegisterInstance(vsInstance);

                return true;
            }

            Console.WriteLine($"No environment variable MSBUILD_EXE_PATH has been set and we were unable to locate it automatically! You need to install SDK for {Environment.Version.ToString(2)}");

            return false;
        }

        private static void SetMsBuildDepsResolver()
        {
            string msbuildDir = Path.GetDirectoryName(Environment.GetEnvironmentVariable("MSBUILD_EXE_PATH"))!;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                AssemblyName name = new AssemblyName(args.Name);

                string assemblyName = $"{name.Name}.dll";
                string sdkFileName = Path.Combine(msbuildDir, assemblyName);

                if (File.Exists(sdkFileName))
                {
                    return Assembly.LoadFile(sdkFileName);
                }

                return null;
            };
        }
    }
}
