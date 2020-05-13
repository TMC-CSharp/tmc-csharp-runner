using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading.Tasks;
using TestMyCode.Csharp.Core.Compiler;
using TestMyCode.Csharp.Core.Test;

namespace TestMyCode.Csharp.Bootstrap
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            string msbuildPath = Environment.GetEnvironmentVariable("MSBUILD_EXE_PATH");
            if (msbuildPath is null)
            {
                Console.WriteLine("No environment variable MSBUILD_EXE_PATH has been set!");

                return;
            }

            RootCommand rootCommand = new RootCommand(description: "TMC helper for running C# projects.")
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
                    "--run-tests-dir"
                }, description: "Sets the directory where the project that should be run is located at.")
                {
                    Argument = new Argument<DirectoryInfo>().ExistingOnly()
                },

                new Option<FileInfo>(aliases: new[]
                {
                    "--output-file",
                    "-o"
                }, description: "The output file used to write results.")
            };

            rootCommand.Handler = CommandHandler.Create(async (bool generatePointsFile, bool runTests, DirectoryInfo runTestsDir, FileInfo outputFile) =>
            {
                if (generatePointsFile)
                {
                    string directory = runTestsDir?.FullName ?? Environment.CurrentDirectory;

                    ProjectCompiler compiler = new ProjectCompiler();
                    ProjectTestPointsFinder points = new ProjectTestPointsFinder();

                    ICollection<string> projects = compiler.CompileTestProjects(directory);

                    foreach (string assemblyPath in projects)
                    {
                        points.FindPoints(assemblyPath);
                    }

                    FileInfo resultsFile = outputFile ?? new FileInfo(Path.Combine(directory, ".tmc_available_points.json"));

                    await WriteToFile(resultsFile, points.Points);
                }

                if (runTests)
                {
                    string directory = runTestsDir?.FullName ?? Environment.CurrentDirectory;

                    ProjectCompiler compiler = new ProjectCompiler();
                    ProjectTestRunner testRunner = new ProjectTestRunner();

                    ICollection<string> projects = compiler.CompileTestProjects(directory);

                    foreach (string assemblyPath in projects)
                    {
                        testRunner.RunTests(assemblyPath);
                    }

                    FileInfo resultsFile = outputFile ?? new FileInfo(Path.Combine(directory, ".tmc_test_results.json"));

                    await WriteToFile(resultsFile, testRunner.TestResults);
                }

                async Task WriteToFile<T>(FileInfo resultsFile, T data)
                {
                    if (resultsFile.Exists)
                    {
                        resultsFile.Delete();
                    }

                    using FileStream stream = resultsFile.Open(FileMode.OpenOrCreate, FileAccess.Write);

                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.WriteIndented = true;

                    await JsonSerializer.SerializeAsync(stream, data, options);
                }
            });

            await rootCommand.InvokeAsync(args);
        }
    }
}
