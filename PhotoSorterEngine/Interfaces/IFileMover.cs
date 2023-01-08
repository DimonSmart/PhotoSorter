namespace PhotoSorterEngine.Interfaces
{
    public interface IFileMover
    {
        public enum FileMoveResultCode
        {
            Ok,
            AlreadyOnPlaceNoMoveRequired,
            AlreadyExist,
            DuplicateNameResultRanamed,
            MoveError,
        }


        record ProgressReport(int Total, int Current, /* int Ok, int Errors,*/ string CurrentFile);

        public FileMoveResult Move(FileMoveParameters options, FileMoveRequest request);

        public IReadOnlyCollection<FileMoveResult> Move(FileMoveParameters options, IReadOnlyCollection<FileMoveRequest> requests, IProgress<ProgressReport>? progressReport = null);
    }
}