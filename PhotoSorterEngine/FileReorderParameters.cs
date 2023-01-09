namespace PhotoSorterEngine
{
    public record FileReorderParameters(
        bool UseCopyInsteadOfMove = false,
        IEnumerable<string>? ComplimentaryFileExtensionsToDelete = null);
}