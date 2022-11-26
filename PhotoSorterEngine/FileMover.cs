using DimonSmart.FileByContentComparer;
using PhotoSorterEngine.Interfaces;
using System.IO.Abstractions;

namespace PhotoSorterEngine
{
    public class FileMover : IFileMover
    {
        private readonly IFileByContentComparer _fileComparer;
        private readonly IFileSystem _fileSystem;
        private FileMoveOptions _options = new();

        public FileMover(IFileByContentComparer fileComparer, IFileSystem fileSystem)
        {
            _fileComparer = fileComparer;
            _fileSystem = fileSystem;
        }

        public void SetOptions(FileMoveOptions options)
        {
            _options = options;
        }

        public FileMoveResult Move(FileMoveRequest request)
        {
            if (request.AlreadyInPlace)
            {
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, "File already in place");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(request.DestinationFileName)!);

            var alreadyExists = _fileSystem.File.Exists(request.DestinationFileName);
            if (!alreadyExists)
            {
                Move(request.SourceFileName, request.DestinationFileName);
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, "Ok");
            }

            // Already existed file exactly the same as original
            if (_fileComparer.Compare(request.SourceFileName, request.DestinationFileName))
            {
                Delete(request.SourceFileName);
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, "The same file already exists");
            }

            // Find different non existed name
            var newDestinationFileName = GetNonExistFileName(request.DestinationFileName);
            Move(request.SourceFileName, newDestinationFileName);
            return new FileMoveResult(request.SourceFileName, newDestinationFileName, $"Already exists. Renamed: {newDestinationFileName}");
        }

        private void Move(string sourceFileName, string destinationFileName)
        {
            if (_options.UseCopyInsteadOfMove)
            {
                _fileSystem.File.Copy(sourceFileName, destinationFileName);
            }
            else
            {
                _fileSystem.File.Move(sourceFileName, destinationFileName);
            }
            
            if (!_options.UseCopyInsteadOfMove && _options.ComplimentaryFileExtensionsToDelete != null)
            {
                foreach (var ext in _options.ComplimentaryFileExtensionsToDelete)
                {
                    Delete(Path.ChangeExtension(sourceFileName, ext));
                }
            }
        }

        private void Delete(string fileName)
        {
            if (_options.UseCopyInsteadOfMove)
            {
                return;
            }
            _fileSystem.File.Delete(fileName);
        }

        public string GetNonExistFileName(string fileName)
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