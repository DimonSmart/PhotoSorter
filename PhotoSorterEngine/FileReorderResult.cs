using static PhotoSorterEngine.Interfaces.IFileReoderer;

namespace PhotoSorterEngine
{
    public record FileReorderResult(
        string Source,
        string Destination,
        FileReorderResultCode ResultCode,
        string? Description = null);
}