using Functional.Maybe;

namespace PhotoSorterEngine
{
    public interface IRenamer
    {
        string Rename(string sourceFileName, DateTime dateTime, string destinationFolder, string pattern);
    }
}