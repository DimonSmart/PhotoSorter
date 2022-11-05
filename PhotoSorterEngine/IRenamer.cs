using Functional.Maybe;

namespace PhotoSorterEngine
{
    public interface IRenamer
    {
        Maybe<string> Rename(string sourceFileName, DateTime dateTime, string destinationFolder, string pattern);
    }
}