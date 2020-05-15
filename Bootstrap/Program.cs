using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using TestMyCode.Csharp.Core.Compiler;
using TestMyCode.Csharp.Core.Data;
using TestMyCode.Csharp.Core.Test;

namespace TestMyCode.Csharp.Bootstrap
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Program.FindMsBuild();

            RootCommand rootCommand = Program.GenerateCommands();
            rootCommand.Handler = CommandHandler.Create(async (bool generatePointsFile, bool runTests, DirectoryInfo projectDir, FileInfo outputFile) =>
            {
                if (!generatePointsFile && !runTests)
                {
                    Console.WriteLine("You didn't give me a task! Use --help for... help! Leave a like if you found out this to be useful!");

                    return;
                }

                string projectDirectory = projectDir?.FullName ?? Environment.CurrentDirectory;

                TestProjectData projectData = new TestProjectData();
                projectData.LoadProjects(projectDirectory);

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

                    using FileStream stream = resultsFile.Open(FileMode.OpenOrCreate, FileAccess.Write);

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

        private static void FindMsBuild()
        {
            string? msbuildPath = Environment.GetEnvironmentVariable("MSBUILD_EXE_PATH");
            if (msbuildPath is null)
            {
                VisualStudioInstance vsInstance = MSBuildLocator.RegisterDefaults();
                if (vsInstance is null)
                {
                    Console.WriteLine("No environment variable MSBUILD_EXE_PATH has been set and we were unable to locate it automatically!");

                    return;
                }

                string msBuildPath = Path.Combine(vsInstance.MSBuildPath, "MSBuild.dll");
                if (!File.Exists(msBuildPath))
                {
                    Console.WriteLine($".NET Core SDK was found in the directory ${msBuildPath} but MSBuild.dll was missing. Please set MSBUILD_EXE_PATH environment variable!");

                    return;
                }

                Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", msBuildPath);
            }
        }
    }
}
