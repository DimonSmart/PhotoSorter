namespace PhotoSorterEngine
{
    public record FileMoveDescription(string originalFileName, string resultFileName, bool AlreadyInPlace, string description);
}