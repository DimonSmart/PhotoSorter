namespace PhotoSorterEngine.Interfaces
{
    public interface IFileMover
    {
        public FileMoveResult Move(FileMoveParameters options, FileMoveRequest request);
    }
}