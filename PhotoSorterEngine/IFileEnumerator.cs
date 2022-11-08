namespace PhotoSorterEngine
{
    using static MediaTypeExtensions;

    public interface IFileEnumerator
    {
        SourceFiles EnumerateFiles(string baseDirectory, MediaType mediaType);
    }
}