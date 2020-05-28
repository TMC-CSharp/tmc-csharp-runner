using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using TestMyCode.CSharp.Core.Data;
using Xunit.Runners;

namespace TestMyCode.CSharp.Core.Test
{
    public class ProjectTestRunner
    {
        public TestProjectData ProjectData { get; }

        private readonly List<MethodTestResult> _TestResults;

        public ProjectTestRunner(TestProjectData projectData)
        {
            this.ProjectData = projectData;

            this._TestResults = new List<MethodTestResult>();
        }

        public void RunAssemblies(IReadOnlyList<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                this.RunAssemblyTests(assembly);
            }
        }

        public void RunAssemblyTests(Assembly assembly)
        {
            using AssemblyLoadContext.ContextualReflectionScope scope = AssemblyLoadContext.EnterContextualReflection(assembly);

            using ManualResetEvent testsCompleted = new ManualResetEvent(false);
            using AssemblyRunner runner = AssemblyRunner.WithoutAppDomain(assembly.Location);

            runner.OnTestFailed += info =>
            {
                this.AddTestResult(MethodTestResult.FromFail(info));
            };

            runner.OnTestPassed += info =>
            {
                this.ProjectData.Points.TryGetValue(info.TestDisplayName, out HashSet<string>? points);

                this.AddTestResult(MethodTestResult.FromSuccess(info, points));
            };

            runner.OnExecutionComplete += info =>
            {
                testsCompleted.Set();
            };

            //This is non blocking call!
            runner.Start();

            //We don't want to exit before all of the tests have ran
            //This will be signaled once all of the tests have been ran
            testsCompleted.WaitOne();

            //OnExecutionComplete is invoked before setting the Status so spin here until it changes
            SpinWait.SpinUntil(() => runner.Status == AssemblyRunnerStatus.Idle);

            //Give up the time slice
            //Travis sometimes fails inside the xUnit thread pool while trying to wait for completion
            //Try to fix this by trying to give the time slice to the thread pool thread
            Thread.Sleep(1);
        }

        private void AddTestResult(MethodTestResult result)
        {
            //We lock here to ensure that we don't have any race conditions
            //Some tests could run in parallel and this is something we need to guard against
            //We could also outright prevent tests from running in parallel but I see this as better and easier option
            lock (this._TestResults)
            {
                this._TestResults.Add(result);
            }
        }

        public IReadOnlyList<MethodTestResult> TestResults => this._TestResults;
    }
}
