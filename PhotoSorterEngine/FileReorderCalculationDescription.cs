namespace PhotoSorterEngine
{
    public record FileReorderCalculationDescription(IReadOnlyCollection<FileMoveRequest> FileMoveRequests,
                                                    IReadOnlyCollection<FileMoveError> Errors);
}