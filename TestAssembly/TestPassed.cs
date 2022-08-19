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