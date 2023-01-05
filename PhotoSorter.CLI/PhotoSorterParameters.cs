namespace PhotoSorter.CLI
{
    public record PhotoSorterParameters (
        string SourceDirectory,
        string DestinationDirectory,
        string NamePattern,
        bool UseFileCreationDateIfNoExif);
}