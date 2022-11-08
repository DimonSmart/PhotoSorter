namespace PhotoSorterEngine
{
    public record SortingCalculationDescription(ICollection<FileMoveDescription> Operations, ICollection<FileMoveError> Errors);
}