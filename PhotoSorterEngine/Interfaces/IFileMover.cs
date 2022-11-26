namespace PhotoSorterEngine.Interfaces
{
    public interface IFileMover
    {
        public FileMoveResult Move(FileMoveRequest request);
    }
}