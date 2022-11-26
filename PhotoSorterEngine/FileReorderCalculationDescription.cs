namespace PhotoSorterEngine
{
    public record FileReorderCalculationDescription(ICollection<FileMoveRequest> FileMoveRequests,
                                                    ICollection<FileMoveError> Errors);
}