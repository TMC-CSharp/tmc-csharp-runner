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

        [Fact]
        public void TestWithoutPoints()
        {

        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public void NotActuallyTestNoPoints()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {

        }
    }
}
