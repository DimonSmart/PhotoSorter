namespace PhotoSorterEngine
{

    public record SortParameters(string DestinationFolder, string NamePattern, bool UseFileCreationDateIfNoExif);
}