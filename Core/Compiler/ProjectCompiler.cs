using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Xunit.Runners;

namespace TestMyCode.Csharp.Core.Compiler
{
    public class ProjectCompiler
    {
        private ProjectCollection ProjectCollection { get; }

        public ProjectCompiler()
        {
            this.ProjectCollection = new ProjectCollection();
        }

        public ICollection<string> CompileTestProjects(string projectPath)
        {
            List<string> files = new List<string>();

            foreach (string projectFile in Directory.EnumerateFiles(projectPath, "*Tests.csproj", SearchOption.AllDirectories))
            {
                Project project = this.ProjectCollection.LoadProject(projectFile);

                string projectRoot = Path.GetDirectoryName(projectFile);

                this.CleanOutput(projectRoot);

                if (!this.CompileTestProject(project))
                {
                    throw new Exception("The compilation failed");
                }
                
                string outputPath = project.GetPropertyValue("OutputPath").Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                string assemblyName = project.GetPropertyValue("AssemblyName");

                string assemblyPath = Path.Combine(projectRoot, outputPath, $"{assemblyName}.dll");

                files.Add(assemblyPath);
            }

            return files;
        }

        private void CleanOutput(string projectRoot)
        {
            string bin = Path.Combine(projectRoot, "bin");
            if (Directory.Exists(bin))
            {
                DirectoryInfo dir = new DirectoryInfo(bin);
                dir.MoveTo(Path.Combine(projectRoot, "bin_OLD")); //Workaround as Delete is non blocking
                dir.Delete(recursive: true);
            }

            string obj = Path.Combine(projectRoot, "obj");
            if (Directory.Exists(obj))
            {
                DirectoryInfo dir = new DirectoryInfo(obj);
                dir.MoveTo(Path.Combine(projectRoot, "obj_OLD")); //Workaround as Delete is non blocking
                dir.Delete(recursive: true);
            }
        }

        private bool CompileTestProject(Project project)
        {
            bool build = project.Build(targets: new[]
            {
                "Clean",
                "Restore",
                "Publish"
            }, loggers: new ILogger[]
            {
                new CompilerOutputLogger()
            });

            return build;
        }

        private class CompilerOutputLogger : ILogger
        {
            public void Initialize(IEventSource eventSource)
            {
                eventSource.ErrorRaised += (sender, args) =>
                {
                    Console.WriteLine("Compilation error: " + args.Message);
                };
            }

            public void Shutdown()
            {
            }

            string ILogger.Parameters { get; set; }

            LoggerVerbosity ILogger.Verbosity
            {
                get => LoggerVerbosity.Diagnostic;
                set { } //Don't allow set
            }
        }
    }
}
