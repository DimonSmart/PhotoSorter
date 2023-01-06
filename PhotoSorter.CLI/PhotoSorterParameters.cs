namespace PhotoSorter.CLI
{
    public record PhotoSorterParameters(
        PhotoSorterActions Action,
        string SourceDirectory,
        string DestinationDirectory,
        string NamePattern,
        bool UseFileCreationDateIfNoExif,
        bool UseCopyInsteadOfMove,
        IEnumerable<string> ComplimentaryFileExtensionsToDelete
        );
}