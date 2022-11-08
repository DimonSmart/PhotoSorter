namespace PhotoSorterEngine
{
    using static MediaTypeExtensions;
    public class FileEnumerator : IFileEnumerator
    {
        public SourceFiles EnumerateFiles(string sourceFolder, MediaType mediaType)
        {
            var extensions = GetFileExtensions(mediaType);
            var files = Directory.GetFiles(sourceFolder, "*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, ReturnSpecialDirectories = false, BufferSize = 1024 * 1024 })
                .Where(file => extensions.Contains(Path.GetExtension(file)))
                .ToList();
            return new SourceFiles(sourceFolder, files);
        }
    }
}