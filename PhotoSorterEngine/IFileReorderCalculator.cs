namespace PhotoSorterEngine
{
    public interface IFileReorderCalculator
    {
        FileReorderCalculationDescription Calculate(SourceFiles sourceFiles, SortParameters sortParameters);
    }
}