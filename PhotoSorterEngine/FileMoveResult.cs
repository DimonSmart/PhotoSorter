using static PhotoSorterEngine.Interfaces.IFileMover;

namespace PhotoSorterEngine
{
    public record FileMoveResult(
        string Source,
        string Destination,
        FileMoveResultCode ResultCode,
        string? Description = null);
}