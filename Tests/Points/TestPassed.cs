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

        public TestPassed()
        {
            this.TestProjectData.LoadAssembly(this.Assembly);
        }

        [Theory]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysPasses), true)]
        [InlineData(typeof(TestAssembly.TestPassed), nameof(TestAssembly.TestPassed.TestThatAlwaysFails), false)]
        public void ClassHasCorrectPassed(Type methodClass, string methodName, bool expect)
        {
            ProjectTestRunner testRunner = new ProjectTestRunner(TestProjectData);

            testRunner.RunAssemblyTests(Assembly);

            bool passed = testRunner.TestResults.FirstOrDefault(result => result.Name == $"{methodClass.FullName}.{methodName}").Passed;

            Assert.Equal(passed, expect);
        }
    }
}