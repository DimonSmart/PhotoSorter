using Functional.Maybe;

namespace PhotoSorterEngine
{
    public interface IFileCreationDatetimeExtractor
    {
        Maybe<DateTime> Extract(string fileName);
        Maybe<DateTime> Extract(Stream fileStream);
    }
}