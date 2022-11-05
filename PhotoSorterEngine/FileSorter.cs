using MetadataExtractor;

namespace PhotoSorterEngine
{
    public class FileSorter : IFileSorter
    {
        public Dictionary<string, string> CalculateSorting(SortParameters sortParameters)
        {
            var sourceFiles = System.IO.Directory.GetFiles(sortParameters.SourceFolder, sortParameters.SearchPattern, SearchOption.AllDirectories).ToList();

            var operations = new Dictionary<string, string>();

            foreach (var file in sourceFiles)
            {
                var destination = CalculateDestinationPath(sortParameters, file);
            }

            return operations;
        }

        string CalculateDestinationPath(SortParameters sortParameters, string fileName)
        {
            var newFileName = GetNewFileName(sortParameters, fileName);
            var destination = Path.Combine(sortParameters.DestinationFolder, newFileName);
            return destination;
        }

        private string GetNewFileName(SortParameters sortParameters, string fileName)
        {
            var directories = ImageMetadataReader.ReadMetadata(fileName);
            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                    Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");

            // Possible options
            // YYYY - FileCreation year as
            return fileName;
        }
    }
}