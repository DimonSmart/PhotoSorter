namespace PhotoSorterEngine
{
    public interface IFileMover
    {
        public FileMoveResult Move(FileMoveRequest request);
    }
}