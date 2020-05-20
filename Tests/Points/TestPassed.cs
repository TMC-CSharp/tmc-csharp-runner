using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using TestMyCode.CSharp.Core.Data;
using TestMyCode.CSharp.Core.Test;
using Xunit;

namespace TestMyCode.CSharp.Tests.Passed
{
    public class TestPassed
    {
        private Assembly Assembly = Assembly.Load("TestAssembly");
        private TestProjectData TestProjectData = new TestProjectData();
        private ProjectTestRunner testRunner;

        public TestPassed()
        {
            this.TestProjectData.LoadAssembly(this.Assembly);

            testRunner = new ProjectTestRunner(TestProjectData);
            testRunner.RunAssemblyTests(Assembly);
        }

        [Theory]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysPasses), true)]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysFails), false)]
        public void MethodResultsCorrectPassed(Type methodClass, string methodName, bool expect)
        {
            bool passed = testRunner.TestResults.FirstOrDefault(result => result.Name == $"{methodClass.FullName}.{methodName}").Passed;

            Assert.Equal(passed, expect);
        }

        [Theory]
        [InlineData(typeof(TestAssembly.TestPassed), "TestThatAlwaysPasses", true)]
        [InlineData(typeof(TestAssembly.TestPassed), "TestThatAlwaysFails", true)]
        [InlineData(typeof(TestAssembly.TestPassed), "TestThatDoesNotExist", false)]
        public void MethodWithGivenNameCanBeFound(Type methodClass, string methodName, bool expect)
        {
            MethodTestResult found = testRunner.TestResults.FirstOrDefault(result => result.Name == $"{methodClass.FullName}.{methodName}");

            Assert.Equal(found != null, expect);
        }

        [Theory]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysFails), true)]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysPasses), false)]
        public void ErrorStackTraceCanBeFoundAfterFailing(Type methodClass, string methodName, bool expect)
        {
            IList<string> stackTrace = testRunner.TestResults.FirstOrDefault(result => result.Name == $"{methodClass.FullName}.{methodName}").ErrorStackTrace;

            Assert.Equal(stackTrace.Count > 0, expect);
        }

        [Theory]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysFails), true)]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysPasses), false)]
        public void MessageCanBeFoundAfterFailing(Type methodClass, string methodName, bool expect)
        {
            string message = testRunner.TestResults.FirstOrDefault(result => result.Name == $"{methodClass.FullName}.{methodName}").Message;

            Assert.Equal(message != "", expect);
        }
    }
}