using System.ComponentModel;

namespace PhotoSorterEngine.Interfaces
{
    public interface IFileReoderer
    {
        public enum FileReorderResultCode
        {
            [Description("Ok")]
            Ok,
            [Description("File already on place")]
            AlreadyOnPlaceNoMoveRequired,
            [Description("Exactly same file already exist in right place")]
            AlreadyExist,
            [Description("Duplicate name, file renamed")]
            DuplicateNameResultRanamed,
            [Description("File copy/move error")]
            MoveError,
        }

        record ProgressReport(int Total, int Current, /* int Ok, int Errors,*/ string CurrentFile);

        public FileReorderResult Reorder(FileReorderParameters options, FileReorderRequest request);

        public IReadOnlyCollection<FileReorderResult> Reorder(FileReorderParameters options, IReadOnlyCollection<FileReorderRequest> requests, IProgress<ProgressReport>? progressReport = null);
    }
}