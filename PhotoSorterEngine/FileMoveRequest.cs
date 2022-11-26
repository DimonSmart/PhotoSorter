namespace PhotoSorterEngine
{
    public record FileMoveRequest(string SourceFileName, string DestinationFileName, bool AlreadyInPlace, string Description);
}