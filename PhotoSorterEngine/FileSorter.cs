using System.Text;
using System.Text.RegularExpressions;

namespace PhotoSorterEngine
{

    public class FileSorter : IFileSorter
    {
        private readonly IRenamer _renamer;
        private readonly IFileCreationDatetimeExtractor _fileCreationDatetimeExtractor;

        public FileSorter(IRenamer renamer, IFileCreationDatetimeExtractor fileCreationDatetimeExtractor)
        {
            _renamer = renamer;
            _fileCreationDatetimeExtractor = fileCreationDatetimeExtractor;
        }

        public SortingCalculationDescription CalculateSorting(SourceFiles sourceFiles, SortParameters sortParameters)
        {
            var operations = new List<FileMoveDescription>();
            var errors = new List<FileMoveError>();
            foreach (var file in sourceFiles.files)
            {
                var dateTime = _fileCreationDatetimeExtractor.Extract(file, sortParameters.UseFileCreationDateIfNoExif);
                if (dateTime.IsSuccess)
                {

                    if (IsAlreadyInPlace(file, dateTime.Value, sortParameters.DestinationFolder, sortParameters.NamePattern, true, out var actualLocation))
                    {
                        operations.Add(new FileMoveDescription(file, actualLocation, true, "On the spot"));
                        continue;
                    }

                    var newFileName = _renamer.Rename(file, dateTime.Value, sortParameters.DestinationFolder, sortParameters.NamePattern);
                    newFileName = RemoveCommentTag(newFileName);

                    // Move needed
                    operations.Add(new FileMoveDescription(file, newFileName, false, string.Empty));
                }
                else
                {
                    errors.Add(new FileMoveError(file, dateTime.Error.Message));
                }
            }

            return new SortingCalculationDescription(operations, errors);
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