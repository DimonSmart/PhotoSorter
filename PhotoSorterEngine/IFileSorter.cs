namespace PhotoSorterEngine
{
    public interface IFileSorter
    {
        SortingCalculationDescription CalculateSorting(SourceFiles sourceFiles, SortParameters sortParameters);
    }
}