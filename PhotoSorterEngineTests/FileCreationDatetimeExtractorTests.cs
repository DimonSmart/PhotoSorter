using FluentAssertions;
using PhotoSorterEngine.Interfaces;
using System.Globalization;
using Xunit.Abstractions;

namespace PhotoSorterEngineTests
{

    public class FileCreationDatetimeExtractorTests : TestsBase
    {
        private readonly IFileCreationDatetimeExtractor _fileCreationDatetimeExtractor;

        public FileCreationDatetimeExtractorTests(ITestOutputHelper testOutputHelper, IFileCreationDatetimeExtractor fileCreationDatetimeExtractor) : base(testOutputHelper)
        {
            _fileCreationDatetimeExtractor = fileCreationDatetimeExtractor;
        }

        [Theory]
        [InlineData(@"TestData\Canon_40D.jpg", @"2008-07-31T10:38:11.0000000")]
        [InlineData(@"TestData\canon_hdr_YES.jpg", @"2015-02-26T04:20:24.0000000")]
        [InlineData(@"TestData\jolla.jpg", @"2014-09-21T16:00:56.0000000")]
        [InlineData(@"TestData\Nikon_D70.JPG", @"2008-07-31T10:03:44.0000000")]
        [InlineData(@"TestData\Sony_HDR-HC3.JPG", @"2008-07-31T17:20:21.0000000")]
        public void ExtractFileDateOkTest(string sourceFileName, string datetime)
        {
            var expectedDateTime = DateTime.Parse(datetime, null, DateTimeStyles.RoundtripKind);
            var result = _fileCreationDatetimeExtractor.Extract(sourceFileName, false);
            result.IsSuccess.Should().Be(true);
            result.Value.Should().Be(expectedDateTime);
        }
    }
}