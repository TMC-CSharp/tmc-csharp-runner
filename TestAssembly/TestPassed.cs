using System;
using System.Collections.Generic;
using System.Text;
using TestMyCode.CSharp.API.Attributes;
using Xunit;

namespace TestMyCode.CSharp.TestAssembly
{
    public class TestPassed
    {
        [Fact]
        public void TestThatAlwaysPasses()
        {
            Assert.True(true);
        }

        [Fact]
        public void TestThatAlwaysFails()
        {
            Assert.True(false);
        }
    }
}