using System;
using System.Collections.Generic;
using System.Text;
using TestMyCode.CSharp.API.Attributes;
using Xunit;

namespace TestMyCode.CSharp.TestAssembly
{
    [Points("1")]
    public class TestPoints
    {
        [Fact]
        [Points("1.3")]
        public void TestWithPoints()
        {

        }

        public void TestWithoutPoints()
        {

        }
    }
}
