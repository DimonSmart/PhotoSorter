namespace PhotoSorterEngine
{
    public record FileReorderRequest(
        string SourceFileName,
        string DestinationFileName,
        bool AlreadyInPlace,
        string Description);
}