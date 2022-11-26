namespace PhotoSorterEngine.Interfaces
{
    using static MediaTypeExtensions;

    public interface IFileEnumerator
    {
        SourceFiles EnumerateFiles(string baseDirectory, MediaType mediaType, int take = 0);
    }
}