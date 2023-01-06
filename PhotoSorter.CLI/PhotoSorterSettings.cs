namespace PhotoSorter.CLI
{
    public class PhotoSorterSettings
    {
        public PhotoSorterActions Action { get; set; }
        public string? SourceDirectory { get; set; }
        public string? DestinationDirectory { get; set; }
        public string? NamePattern { get; set; }
        public bool UseFileCreationDateIfNoExif { get; set; }
        public bool UseCopyInsteadOfMove { get; set; }
        public IEnumerable<string>? ComplimentaryFileExtensionsToDelete { get; set; }
    }
}