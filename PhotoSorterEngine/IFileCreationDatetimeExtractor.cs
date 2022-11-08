using Functional.Maybe;
using ResultMonad;

namespace PhotoSorterEngine
{
    public interface IFileCreationDatetimeExtractor
    {
        Result<DateTime, Exception> Extract(string fileName, bool UseFileCreationDateIfNoExif);
    }
}