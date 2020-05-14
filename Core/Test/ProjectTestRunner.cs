﻿using System;
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
            ProjectTestPointsFinder pointsFinder = new ProjectTestPointsFinder();
            pointsFinder.FindPoints(assemblyPath);

            using ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            using AssemblyRunner runner = AssemblyRunner.WithoutAppDomain(assembly.Location);

            string[] points = new string[pointsFinder.Points.Count];
            int i = 0;
            foreach (HashSet<string> pointSet in pointsFinder.Points.Values)
            {
                foreach (string point in pointSet)
                {
                    points[i] = point;
                    i++;
                }
            }

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

                        Name = info.TestDisplayName,

                        Points = points, 

                        Message = "test" // unfinished
                    });
                }
            };

            runner.Start();

            manualResetEvent.WaitOne();
        }

        public IReadOnlyList<MethodTestResult> TestResults => this._TestResults;
    }
}
