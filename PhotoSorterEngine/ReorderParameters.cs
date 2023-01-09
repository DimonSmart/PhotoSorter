namespace PhotoSorterEngine
{
    public record ReorderParameters(
        string DestinationFolder,
        string NamePattern,
        bool UseFileCreationDateIfNoExif);
}