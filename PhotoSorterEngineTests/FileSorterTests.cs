using FluentAssertions;
using PhotoSorterEngine;
using Xunit.Abstractions;
using static PhotoSorterEngine.MediaTypeExtensions;

namespace PhotoSorterEngineTests
{
    public class FileReorderCalculatorTests : TestsBase
    {
        public FileReorderCalculatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        // [Fact]
        public void FileSorterPositiveTest()
        {
            var fileCreationDatetimeExtractor = new FileCreationDatetimeExtractor();
            var renamer = new Renamer();
            var fileSorter = new FileReorderCalculator(renamer, fileCreationDatetimeExtractor);
            var fileEnumerator = new FileEnumerator();
            var sourceFiles = fileEnumerator.EnumerateFiles(@"X:\PhotoArchive", MediaType.All);
            var result = fileSorter.Calculate(sourceFiles,
                new SortParameters(@"X:\Photos", @"%YYYY%\%Comment%%YYYY%-%MM%-%DD%%Comment%", UseFileCreationDateIfNoExif: false));
            File.WriteAllLines(@"C:\temp\1.txt", result.Operations.Where(r => !r.AlreadyInPlace).Select(s => s.SourceFileName + " " + s.DestinationFileName));
            File.WriteAllLines(@"C:\temp\2.txt", result.Errors.Select(s => s.OriginalFileName + " " + s.Error));
        }
    }
}