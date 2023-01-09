namespace PhotoSorterEngine
{
    public record FileReorderCalculationDescription(IReadOnlyCollection<FileReorderRequest> FileReorderRequests,
                                                    IReadOnlyCollection<FileReorderError> Errors);
}