using System.Text;
using System.Text.RegularExpressions;

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
            var fileMoveRequests = new List<FileMoveRequest>();
            var fileMoveRequestsWithErrors = new List<FileMoveError>();
            var currentFileNumber = 1;
            foreach (var file in sourceFiles.files)
            {
                progressReport?.Report(
                    new IFileReorderCalculator.ProgressReport(
                        Total: sourceFiles.files.Count,
                        Current: currentFileNumber,
                        Ok: fileMoveRequests.Count,
                        Errors: fileMoveRequestsWithErrors.Count,
                        CurrentFile: file));
                var dateTime = _fileCreationDatetimeExtractor.Extract(file, sortParameters.UseFileCreationDateIfNoExif);
                if (dateTime.IsSuccess)
                {

                    if (IsAlreadyInPlace(file, dateTime.Value, sortParameters.DestinationFolder, sortParameters.NamePattern, true, out var actualLocation))
                    {
                        fileMoveRequests.Add(new FileMoveRequest(file, actualLocation, true, "On the spot"));
                        continue;
                    }

                    var newFileName = _renamer.Rename(file, dateTime.Value, sortParameters.DestinationFolder, sortParameters.NamePattern);
                    newFileName = RemoveCommentTag(newFileName);

                    // Move needed
                    fileMoveRequests.Add(new FileMoveRequest(file, newFileName, false, string.Empty));
                }
                else
                {
                    fileMoveRequestsWithErrors.Add(new FileMoveError(file, dateTime.Error.Message));
                }
            }

            return new FileReorderCalculationDescription(fileMoveRequests, fileMoveRequestsWithErrors);
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

        private bool IsAlreadyInPlace(string file, DateTime dateTime, string destinationFolder, string namePattern, out string actualLocation)
        {
            var newFileName = _renamer.Rename(file, dateTime, destinationFolder, namePattern);
            var parts = newFileName.Split("%Comment%").Select(s => Regex.Escape(s));
            var sb = new StringBuilder();
            var pattern = string.Join(".*", parts);
            var match = Regex.Match(file, pattern);
            actualLocation = newFileName;
            return match.Success;
        }
    }
}