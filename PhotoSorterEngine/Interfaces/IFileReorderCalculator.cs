namespace PhotoSorterEngine.Interfaces
{
    public interface IFileReorderCalculator
    {
        record ProgressReport(int Total, int Current, int Ok, int Errors, string CurrentFile);
        FileReorderCalculationDescription Calculate(SourceFiles sourceFiles, SortParameters sortParameters, IProgress<ProgressReport>? progressReport = null);
    }
}