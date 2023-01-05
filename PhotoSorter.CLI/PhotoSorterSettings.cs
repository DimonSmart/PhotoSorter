namespace PhotoSorter.CLI
{
    public class PhotoSorterSettings
    {
        public string? SourceDirectory { get; set; }
        public string? DestinationDirectory { get; set; }
        public string? NamePattern { get; set; }
        public bool UseFileCreationDateIfNoExif { get; set; }
    }
}