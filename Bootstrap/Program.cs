using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
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

                new Option<DirectoryInfo>(aliases: new[]
                {
                    "--run-tests",
                    "-t"
                }, description: "Run tests for the project.")
                {
                    Argument = new Argument<DirectoryInfo>().ExistingOnly()
                },

                new Option<FileInfo>(aliases: new[]
                {
                    "--output-file",
                    "-o"
                }, description: "The output file used to write results", getDefaultValue: () => new FileInfo("results.json"))
            };

            rootCommand.Handler = CommandHandler.Create(async (bool generatePointsFile, DirectoryInfo runTests, FileInfo outputFile) =>
            {
                if (generatePointsFile)
                {
                    //TODO
                }

                if (!(runTests is null))
                {
                    ProjectCompiler compiler = new ProjectCompiler();
                    ProjectTestRunner testRunner = new ProjectTestRunner();

                    ICollection<string> projects = compiler.CompileTestProjects(runTests.FullName);

                    foreach (string assemblyPath in projects)
                    {
                        testRunner.RunTests(assemblyPath);
                    }

                    using FileStream stream = outputFile.Open(FileMode.OpenOrCreate, FileAccess.Write);

                    await JsonSerializer.SerializeAsync(stream, testRunner.TestResults);
                }
            });

            await rootCommand.InvokeAsync(args);
        }
    }
}
