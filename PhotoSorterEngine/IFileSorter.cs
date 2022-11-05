namespace PhotoSorterEngine
{
    public interface IFileSorter
    {
        Dictionary<string, string> CalculateSorting(SortParameters sortParameters);
    }
}