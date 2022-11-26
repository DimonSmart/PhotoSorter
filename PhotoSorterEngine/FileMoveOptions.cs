namespace PhotoSorterEngine
{
    public record FileMoveOptions(bool UseCopyInsteadOfMove = false, IEnumerable<string>? ComplimentaryFileExtensionsToDelete = null);
}