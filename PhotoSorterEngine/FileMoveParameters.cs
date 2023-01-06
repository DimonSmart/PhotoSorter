namespace PhotoSorterEngine
{
    public record FileMoveParameters(
        bool UseCopyInsteadOfMove = false,
        IEnumerable<string>? ComplimentaryFileExtensionsToDelete = null);
}