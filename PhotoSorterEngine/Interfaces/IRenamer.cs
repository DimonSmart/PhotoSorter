namespace PhotoSorterEngine.Interfaces
{
    public interface IRenamer
    {
        string Rename(string sourceFileName, DateTime dateTime, string destinationFolder, string pattern);
    }
}