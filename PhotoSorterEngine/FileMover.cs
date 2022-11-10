namespace PhotoSorterEngine
{
    public class FileMover : IFileMover
    {
        private readonly IFileByContentComparer _fileComparer;

        public FileMover(IFileByContentComparer fileComparer)
        {
            _fileComparer = fileComparer;
        }

        public FileMoveResult Move(FileMoveRequest request)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(request.DestinationFileName)!);

            var alreadyExists = File.Exists(request.DestinationFileName);
            if (!alreadyExists)
            {
                File.Move(request.SourceFileName, request.DestinationFileName);
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, "Ok");
            }

            // Already existed file exactly the same as original
            if (_fileComparer.Compare(request.SourceFileName, request.DestinationFileName))
            {
                File.Delete(request.SourceFileName);
                return new FileMoveResult(request.SourceFileName, request.DestinationFileName, "The same file already exists");
            }

            // Find different non existed name
            var newDestinationFileName = GetNonExistFileName(request.DestinationFileName);
            File.Move(request.SourceFileName, newDestinationFileName);
            return new FileMoveResult(request.SourceFileName, newDestinationFileName, $"Already exists. Renamed: {newDestinationFileName}");
        }

        public static string GetNonExistFileName(string fileName)
        {
            int count = 1;
            var fileNameOnly = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var path = Path.GetDirectoryName(fileName) ?? string.Empty;
            var newFullPath = fileName;

            while (File.Exists(newFullPath))
            {
                var tempFileName = Path.ChangeExtension($"{fileNameOnly}_{count}", extension);
                newFullPath = Path.Combine(path, tempFileName);
            };
            return newFullPath;
        }
    }
}