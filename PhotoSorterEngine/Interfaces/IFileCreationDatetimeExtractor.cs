using ResultMonad;

namespace PhotoSorterEngine.Interfaces
{
    public interface IFileCreationDatetimeExtractor
    {
        Result<DateTime, Exception> Extract(string fileName, bool UseFileCreationDateIfNoExif);
    }
}