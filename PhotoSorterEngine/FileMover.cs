using DimonSmart.FileByContentComparer;
using PhotoSorterEngine.Interfaces;
using System.IO.Abstractions;
using static PhotoSorterEngine.Interfaces.IFileMover.FileMoveResultCode;

namespace PhotoSorterEngine
{
    public class FileMover : IFileMover
    {
        private readonly IFileByContentComparer _fileComparer;
        private readonly IFileSystem _fileSystem;

        public FileMover(IFileByContentComparer fileComparer, IFileSystem fileSystem)
        {
            _fileComparer = fileComparer;
            _fileSystem = fileSystem;
        }

        public FileMoveResult Move(FileMoveParameters options, FileMoveRequest request)
        {
            if (request.AlreadyInPlace)
            {
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, AlreadyOnPlaceNoMoveRequired);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(request.DestinationFileName)!);

            var alreadyExists = _fileSystem.File.Exists(request.DestinationFileName);
            if (!alreadyExists)
            {
                Move(options, request.SourceFileName, request.DestinationFileName);
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, Ok);
            }

            // Already existed file exactly the same as original
            if (_fileComparer.Compare(request.SourceFileName, request.DestinationFileName))
            {
                Delete(options, request.SourceFileName);
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, AlreadyExist);
            }

            // Find different non existed name
            var newDestinationFileName = GetNonExistFileName(request.DestinationFileName);
            Move(options, request.SourceFileName, newDestinationFileName);
            return new FileMoveResult(request.SourceFileName, newDestinationFileName, DuplicateNameResultRanamed, $"Already exists. Renamed: {newDestinationFileName}");
        }

        public IReadOnlyCollection<FileMoveResult> Move(FileMoveParameters options, IReadOnlyCollection<FileMoveRequest> requests, IProgress<IFileMover.ProgressReport>? progressReport = null)
        {
            var results = new List<FileMoveResult>();
            var currentFileNumber = 1;
            foreach (var request in requests)
            {
                var moveResult = Move(options, request);
                results.Add(moveResult);
                progressReport?.Report(
                    new IFileMover.ProgressReport(
                        Total: requests.Count,
                        Current: currentFileNumber++,
                        CurrentFile: request.SourceFileName));
            }
            return results;
        }

        private void Move(FileMoveParameters options, string sourceFileName, string destinationFileName)
        {
            if (options.UseCopyInsteadOfMove)
            {
                _fileSystem.File.Copy(sourceFileName, destinationFileName);
            }
            else
            {
                _fileSystem.File.Move(sourceFileName, destinationFileName);
            }

            if (!options.UseCopyInsteadOfMove && options.ComplimentaryFileExtensionsToDelete != null)
            {
                foreach (var ext in options.ComplimentaryFileExtensionsToDelete)
                {
                    Delete(options, Path.ChangeExtension(sourceFileName, ext));
                }
            }
        }

        private void Delete(FileMoveParameters options, string fileName)
        {
            if (options.UseCopyInsteadOfMove)
            {
                return;
            }
            _fileSystem.File.Delete(fileName);
        }

        internal string GetNonExistFileName(string fileName)
        {
            int count = 1;
            var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var path = Path.GetDirectoryName(fileName) ?? string.Empty;
            var newFullPath = fileName;

            while (_fileSystem.File.Exists(newFullPath))
            {
                var tempFileName = Path.ChangeExtension($"{fileNameOnly}_{count}", extension);
                newFullPath = Path.Combine(path, tempFileName);
            };
            return newFullPath;
        }
    }
}