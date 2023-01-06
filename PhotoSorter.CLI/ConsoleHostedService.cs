using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoSorterEngine.Interfaces;
using PhotoSorterEngine;
using Microsoft.Extensions.Options;
using static PhotoSorterEngine.MediaTypeExtensions;
using DimonSmart.FileByContentComparer;
using ResultMonad;
using System.IO;
using XmpCore.Options;

namespace PhotoSorter.CLI
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IOptions<PhotoSorterSettings> _photoSorterSettings;
        private readonly IFileEnumerator _fileEnumerator;
        private readonly IFileReorderCalculator _fileReorderCalculator;
        private readonly IFileMover _fileMover;
        private int? _exitCode;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IOptions<PhotoSorterSettings> photoSorterSettings,
            IFileEnumerator fileEnumerator,
            IFileReorderCalculator fileReorderCalculator,
            IFileMover fileMover)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _photoSorterSettings = photoSorterSettings;
            _fileEnumerator = fileEnumerator;
            _fileReorderCalculator = fileReorderCalculator;
            _fileMover = fileMover;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting with arguments:'{Arguments}'", string.Join(" ", Environment.GetCommandLineArgs()));

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        DoSort();
                        _exitCode = 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                        _exitCode = 1;
                    }
                    finally
                    {
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        private void DoSort()
        {
            _logger.LogInformation("PhotoSorter started!");

            var parameters = GetPhotoSorterParameters(_photoSorterSettings);
            ShowPhotoSorterParameters(parameters);

            if (parameters.Action == PhotoSorterActions.Help)
            {
                ShowHelp();
                return;
            }

            var sourceFiles = _fileEnumerator.EnumerateFiles(parameters.SourceDirectory, MediaType.All);
            _logger.LogInformation("Files to sort:'{FilesCount}'", sourceFiles.Files.Count);

            var fileReorderCalculationDescription = _fileReorderCalculator
                .Calculate(sourceFiles,
                           new SortParameters(
                               parameters.DestinationDirectory,
                               parameters.NamePattern,
                               parameters.UseFileCreationDateIfNoExif),
                               new Progress<IFileReorderCalculator.ProgressReport>(ProgressIndicator));
            LogFileReorderCalculationErrors(fileReorderCalculationDescription);

            if (parameters.Action == PhotoSorterActions.TestOnly)
            {
                return;
            }

            var fileMoveParameters = new FileMoveParameters
            {
                UseCopyInsteadOfMove = parameters.UseCopyInsteadOfMove,
                ComplimentaryFileExtensionsToDelete = parameters.ComplimentaryFileExtensionsToDelete
            };

            var moveResults = new List<FileMoveResult>();
            foreach (var item in fileReorderCalculationDescription.FileMoveRequests)
            {
                moveResults.Add(_fileMover.Move(fileMoveParameters, item));
            }




        }


        private const string HelpText =
$@" PhotoSorter.CLI.exe
{nameof(PhotoSorterActions.Help)}
Mo more than this text.

{nameof(PhotoSorterActions.TestOnly)}
This action perform all preparations and calculations, except
the real files (copying/moving).

{nameof(PhotoSorterActions.Sort)}
This action perform files sorting.
We recomment to set option UseCopy to perform copy-style photo sorting.

The are two main usage styles:
First: Source directory pointed to removable drive, Destination - pointed to you photo storage
In this case application import and sort photos by dates.

Second: Source and destination directories point the same folder.
In this case we perform onplace sorting.
It's better to use move style operation option to avoid files duplication

Main parameters:
    ""Action"" - [{nameof(PhotoSorterActions.TestOnly)}, {nameof(PhotoSorterActions.Sort)}]
    ""SourceDirectory"" - Full path to source folder. Example: ""D:\\DCIM\\"",
    ""DestinationDirectory"" - Full path to destination folder. Example: ""C:\\Photos\\"",
    ""NamePattern"" - Destination subfolders naming strategy. Example: ""%YYYY%\\%Comment%%YYYY%-%MM%-%DD%%Comment%"",
    ""UseFileCreationDateIfNoExif"" - If it's impossible to extract file creation date - use filesystem file creation date
";
        private void ShowHelp()
        {
            _logger.LogInformation(HelpText);
        }

        private const int ProgressDevider = 50;
        void ProgressIndicator(IFileReorderCalculator.ProgressReport r)
        {
            if (r.Current != 1 && r.Current != r.Total && r.Current % ProgressDevider != 0)
            {
                return;
            }

            _logger.LogInformation("{Current} of {Total}", r.Current, r.Total);
        }

        private void LogFileReorderCalculationErrors(FileReorderCalculationDescription result)
        {
            if (result.Errors.Count > 0)
            {
                _logger.LogInformation("Files sort completed with:'{ErrorsCount}' errors", result.Errors.Count);
                _logger.LogInformation("Errors samples:'{Errors}'", result.Errors.Select(e => $"{e}{Environment.NewLine}").Take(10));
            }
            else
            {
                _logger.LogInformation("Files sort completed without errors");
            }
        }

        internal static PhotoSorterParameters GetPhotoSorterParameters(IOptions<PhotoSorterSettings> photoSorterSettings)
        {
            var sourceDirectory = photoSorterSettings.Value.SourceDirectory;
            if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
            {
                throw new Exception("Source folder must be specified and exists!");
            }

            var destinationDirectory = photoSorterSettings.Value.DestinationDirectory;
            if (string.IsNullOrWhiteSpace(destinationDirectory) || !Directory.Exists(destinationDirectory))
            {
                throw new Exception("Destination folder (root) must be specified and exists!");
            }

            var namePattern = photoSorterSettings.Value.NamePattern;
            if (string.IsNullOrEmpty(namePattern))
            {
                throw new Exception("Name pattern must be specified!");
            }

            var useFileCreationDateIfNoExif = photoSorterSettings.Value.UseFileCreationDateIfNoExif;

            var action = photoSorterSettings.Value.Action;
            if (!Enum.IsDefined(action))
            {
                action = PhotoSorterActions.Help;
            }

            var useCopyInsteadOfMove = photoSorterSettings.Value.UseCopyInsteadOfMove;

            var complimentaryFileExtensionsToDelete = photoSorterSettings.Value.ComplimentaryFileExtensionsToDelete ?? Array.Empty<string>();

            return new PhotoSorterParameters(
                action,
                sourceDirectory,
                destinationDirectory,
                namePattern,
                useFileCreationDateIfNoExif,
                useCopyInsteadOfMove,
                complimentaryFileExtensionsToDelete);
        }

        public void ShowPhotoSorterParameters(PhotoSorterParameters parameters)
        {
            var launchParameters = $@"
{nameof(PhotoSorterParameters.Action)}:'{parameters.Action}'
{nameof(PhotoSorterParameters.SourceDirectory)}:'{parameters.SourceDirectory}'
{nameof(PhotoSorterParameters.DestinationDirectory)}:'{parameters.DestinationDirectory}'
{nameof(PhotoSorterParameters.NamePattern)}:'{parameters.NamePattern}'
{nameof(PhotoSorterParameters.UseFileCreationDateIfNoExif)}:'{parameters.UseFileCreationDateIfNoExif}'
{nameof(PhotoSorterParameters.UseCopyInsteadOfMove)}:'{parameters.UseCopyInsteadOfMove}'
{nameof(PhotoSorterParameters.ComplimentaryFileExtensionsToDelete)}:'{string.Join(", ", parameters.ComplimentaryFileExtensionsToDelete)}'";

            _logger.LogInformation("Launch parameters: {PhotoSorterParameters}", launchParameters);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Exiting with return code: {ExitCode}", _exitCode);

            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }
    }
}