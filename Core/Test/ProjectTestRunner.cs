using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Xunit.Runners;

namespace TestMyCode.Csharp.Core.Test
{
    public class ProjectTestRunner
    {
        private List<MethodTestResult> _TestResults { get; }

        public ProjectTestRunner()
        {
            this._TestResults = new List<MethodTestResult>();
        }

        public void RunTests(string assemblyPath)
        {
            //I would prefer using AssemblyLoadContext but that does not seem to work here, oh well
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            using ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            using AssemblyRunner runner = AssemblyRunner.WithoutAppDomain(assembly.Location);

            runner.OnExecutionComplete += info =>
            {
                manualResetEvent.Set();
            };

            runner.OnTestFailed += info =>
            {
                lock(this._TestResults)
                {
                    this._TestResults.Add(new MethodTestResult()
                    {
                        Passed = false,

                        Name = info.TestDisplayName,
                        ErrorStackTrace = info.ExceptionStackTrace
                    });
                }
            };

            runner.OnTestPassed += info =>
            {
                lock (this._TestResults)
                {
                    this._TestResults.Add(new MethodTestResult()
                    {
                        Passed = true,

                        Name = info.TestDisplayName
                    });
                }
            };

            runner.Start();

            manualResetEvent.WaitOne();
        }

        public IReadOnlyList<MethodTestResult> TestResults => this._TestResults;
    }
}
