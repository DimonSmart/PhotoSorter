using System.Text.RegularExpressions;
using PhotoSorterEngine.Interfaces;

namespace PhotoSorterEngine
{
    public class FileReorderCalculator : IFileReorderCalculator
    {
        private readonly IRenamer _renamer;
        private readonly IFileCreationDatetimeExtractor _fileCreationDatetimeExtractor;

        public FileReorderCalculator(IRenamer renamer, IFileCreationDatetimeExtractor fileCreationDatetimeExtractor)
        {
            _renamer = renamer;
            _fileCreationDatetimeExtractor = fileCreationDatetimeExtractor;
        }

        public FileReorderCalculationDescription Calculate(SourceFiles sourceFiles, SortParameters sortParameters, IProgress<IFileReorderCalculator.ProgressReport>? progressReport = null)
        {
            var fileMoveCalculationResults = new List<FileMoveRequest>();
            var fileMoveRequestsWithErrors = new List<FileMoveError>();
            var currentFileNumber = 1;
            foreach (var file in sourceFiles.Files)
            {
                progressReport?.Report(
                    new IFileReorderCalculator.ProgressReport(
                        Total: sourceFiles.Files.Count,
                        Current: currentFileNumber++,
                        Ok: fileMoveCalculationResults.Count,
                        Errors: fileMoveRequestsWithErrors.Count,
                        CurrentFile: file));
                var dateTime = _fileCreationDatetimeExtractor.Extract(file, sortParameters.UseFileCreationDateIfNoExif);
                if (dateTime.IsSuccess)
                {

                    if (IsAlreadyInPlace(file, dateTime.Value, sortParameters.DestinationFolder, sortParameters.NamePattern, true, out var actualLocation))
                    {
                        fileMoveCalculationResults.Add(new FileMoveRequest(file, actualLocation, true, "On the spot"));
                        continue;
                    }

                    var newFileName = _renamer.Rename(file, dateTime.Value, sortParameters.DestinationFolder, sortParameters.NamePattern);
                    newFileName = RemoveCommentTag(newFileName);

                    // Move needed
                    fileMoveCalculationResults.Add(new FileMoveRequest(file, newFileName, false, string.Empty));
                }
                else
                {
                    fileMoveRequestsWithErrors.Add(new FileMoveError(file, dateTime.Error.Message));
                }
            }

            return new FileReorderCalculationDescription(fileMoveCalculationResults, fileMoveRequestsWithErrors);
        }

        private static string RemoveCommentTag(string path)
        {
            return path.Replace("%Comment%", string.Empty);
        }

        private bool IsAlreadyInPlace(string file, DateTime dateTime, string destinationFolder, string namePattern, bool oneDayTolerant, out string actualLocation)
        {
            if (IsAlreadyInPlace(file, dateTime, destinationFolder, namePattern, out actualLocation))
            {
                return true;
            }

            if (!oneDayTolerant)
            {
                return false;
            }

            // Do not replace with simple return! Check out variable!  
            if (IsAlreadyInPlace(file, dateTime.AddDays(1), destinationFolder, namePattern, out actualLocation) ||
                IsAlreadyInPlace(file, dateTime.AddDays(-1), destinationFolder, namePattern, out actualLocation))
                return true;
            else
                return false;
        }

        private bool IsAlreadyInPlace(string fileName, DateTime dateTime, string destinationFolder, string namePattern, out string actualLocation)
        {
            actualLocation = fileName;
            var newFileName = _renamer.Rename(fileName, dateTime, destinationFolder, namePattern);
            var parts = newFileName.Split("%Comment%").Select(s => Regex.Escape(s));
            var pattern = string.Join(".*", parts);
            var match = Regex.Match(fileName, pattern);
            if (match.Success)
            {
                actualLocation = match.Value;
            }
            return match.Success;
        }
    }
}