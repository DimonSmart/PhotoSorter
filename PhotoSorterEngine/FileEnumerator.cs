namespace PhotoSorterEngine
{
    using static MediaTypeExtensions;
    public class FileEnumerator : IFileEnumerator
    {
        public SourceFiles EnumerateFiles(string sourceFolder, MediaType mediaType, int take = 0)
        {
            var extensions = GetFileExtensions(mediaType);
            var files = Directory.GetFiles(sourceFolder, "*", new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, ReturnSpecialDirectories = false, BufferSize = 1024 * 1024 })
                .Where(file => extensions.Contains(Path.GetExtension(file), StringComparer.InvariantCultureIgnoreCase));
            if (take > 0)
            {
                files = files.Take(take);
            };
            return new SourceFiles(sourceFolder, files.ToList());
        }
    }
}