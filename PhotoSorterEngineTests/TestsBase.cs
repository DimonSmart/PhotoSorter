using Xunit.Abstractions;

namespace PhotoSorterEngineTests
{
    public class TestsBase
    {
        protected readonly ITestOutputHelper _testOutputHelper;
        public TestsBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
    }
}