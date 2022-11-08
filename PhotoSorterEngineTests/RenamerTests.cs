using FluentAssertions;
using PhotoSorterEngine;
using System.Globalization;
using Xunit.Abstractions;

namespace PhotoSorterEngineTests
{

    public class RenamerTests : TestsBase
    {
        private IRenamer _renamer;
        private DateTime _dateTime = DateTime.Parse(@"2008-07-31T10:38:11.0000000", null, DateTimeStyles.RoundtripKind);


        public RenamerTests(ITestOutputHelper testOutputHelper, IRenamer renamer) : base(testOutputHelper)
        {
            _renamer = renamer;
        }

        [Theory]
        [InlineData(@"X:\TestData\Photos\Heap\Canon_40D.jpg", @"%YYYY%\%MM%\%DD%", @"X:\TestData\Photos\2008\07\31\Canon_40D.jpg")]
        [InlineData(@"X:\TestData\Photos\Heap\Canon_40D.jpg", @"%YYYY%.%MM%.%DD%", @"X:\TestData\Photos\2008.07.31\Canon_40D.jpg")]
        [InlineData(@"X:\TestData\Photos\Heap\Canon_40D.jpg", @"%YYYY%\%MM%\%YYYY%.%MM%.%DD%", @"X:\TestData\Photos\2008\07\2008.07.31\Canon_40D.jpg")]
        [InlineData(@"X:\TestData\Photos\Heap\Canon_40D.jpg", @"%YYYY%\%MMM_ENG%\%DD%", @"X:\TestData\Photos\2008\Jul\31\Canon_40D.jpg")]
        [InlineData(@"X:\TestData\Photos\Heap\Canon_40D.jpg", @"%YYYY%\%MMMM_ENG%\%DD%", @"X:\TestData\Photos\2008\July\31\Canon_40D.jpg")]
        [InlineData(@"X:\TestData\Photos\Heap\Canon_40D.jpg", @"%YYYY%\%MMMM_ENG_LOWER%\%DD%", @"X:\TestData\Photos\2008\july\31\Canon_40D.jpg")]
        public void RenameOkTest(string sourceFileName, string namePattern, string expectedFileName)
        {
            var result = _renamer.Rename(sourceFileName, _dateTime, @"X:\TestData\Photos", namePattern);
            result.Should().BeEquivalentTo(expectedFileName);
        }
    }
}