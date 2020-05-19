using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TestMyCode.CSharp.Core.Data;
using Xunit;

namespace TestMyCode.CSharp.Tests.Points
{
    public class TestPoints
    {
        private Assembly Assembly = Assembly.Load("TestAssembly");
        private TestProjectData TestProjectData = new TestProjectData();

        public TestPoints()
        {
            this.TestProjectData.LoadAssembly(this.Assembly);
        }

        [Theory]
        [InlineData(typeof(TestAssembly.TestPoints), nameof(TestAssembly.TestPoints.TestWithPoints), new[] { "1", "1.3" })]
        [InlineData(typeof(TestAssembly.TestPoints), nameof(TestAssembly.TestPoints.TestWithoutPoints), new[] { "1" })]
        public void ClassHasPoints(Type methodClass, string methodName, string[] expect)
        {
            HashSet<string> points = this.TestProjectData.Points[$"{methodClass.FullName}.{methodName}"];

            Assert.Equal(points, expect);
        }
    }
}
