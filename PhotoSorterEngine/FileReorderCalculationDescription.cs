namespace PhotoSorterEngine
{
    public record FileReorderCalculationDescription(ICollection<FileMoveRequest> Operations, ICollection<FileMoveError> Errors);
}