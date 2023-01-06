using DimonSmart.FileByContentComparer;
using FluentAssertions;
using PhotoSorterEngine;
using System.IO.Abstractions;
using Xunit.Abstractions;
using static PhotoSorterEngine.MediaTypeExtensions;

namespace PhotoSorterEngineTests
{
    public class FileReorderCalculatorTests : TestsBase
    {
        public FileReorderCalculatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact(Skip = "Check file locations before run")]
        public void FileSorterPositiveTest()
        {
            var fileCreationDatetimeExtractor = new FileCreationDatetimeExtractor(new FileSystem());
            var renamer = new Renamer();
            var sourceFiles = new FileEnumerator()
                .EnumerateFiles(@"F:\DCIM", MediaType.All);

            var result = new FileReorderCalculator(renamer, fileCreationDatetimeExtractor)
                .Calculate(sourceFiles,
                           new SortParameters(
                               @"D:\Temp",
                               @"%YYYY%\%Comment%%YYYY%-%MM%-%DD%%Comment%",
                               UseFileCreationDateIfNoExif: false));

            File.WriteAllLines(@"C:\temp\1.txt", result.FileMoveRequests.Where(r => !r.AlreadyInPlace).Select(s => s.SourceFileName + " " + s.DestinationFileName));
            File.WriteAllLines(@"C:\temp\2.txt", result.Errors.Select(s => s.OriginalFileName + " " + s.Error));

            var fileMover = new FileMover(new FileByContentComparer(), new FileSystem());
            var options = new FileMoveParameters
            {
                UseCopyInsteadOfMove = true,
                ComplimentaryFileExtensionsToDelete = new List<string> { ".aac" }
            };

            var moveResults = new List<FileMoveResult>();
            foreach (var item in result.FileMoveRequests)
            {
                moveResults.Add(fileMover.Move(options, item));
            }

            File.WriteAllLines(@"C:\temp\3.txt", moveResults.Select(s => s.Source + " -> " + s.Destination + " = " + s.Description));
        }
    }
}