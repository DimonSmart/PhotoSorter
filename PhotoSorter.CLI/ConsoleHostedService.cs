using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoSorterEngine.Interfaces;
using PhotoSorterEngine;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using static PhotoSorterEngine.MediaTypeExtensions;
using System.Reflection.Emit;
using System;

namespace PhotoSorter.CLI
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IOptions<PhotoSorterSettings> _photoSorterSettings;
        private readonly IFileEnumerator _fileEnumerator;
        private readonly IFileReorderCalculator _fileReorderCalculator;
        private int? _exitCode;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IOptions<PhotoSorterSettings> photoSorterSettings,
            IFileEnumerator fileEnumerator,
            IFileReorderCalculator fileReorderCalculator)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _photoSorterSettings = photoSorterSettings;
            _fileEnumerator = fileEnumerator;
            _fileReorderCalculator = fileReorderCalculator;
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

            var sourceFiles = _fileEnumerator.EnumerateFiles(parameters.SourceDirectory, MediaType.All);
            _logger.LogInformation("Files to sort:'{FilesCount}'", sourceFiles.Files.Count);

            var result = _fileReorderCalculator
                .Calculate(sourceFiles,
                           new SortParameters(
                               parameters.DestinationDirectory,
                               parameters.NamePattern,
                               parameters.UseFileCreationDateIfNoExif),
                               new Progress<IFileReorderCalculator.ProgressReport>(ProgressIndicator));
            LogFileReorderCalculationErrors(result);
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

        private PhotoSorterParameters GetPhotoSorterParameters(IOptions<PhotoSorterSettings> photoSorterSettings)
        {
            var sourceDirectory = _photoSorterSettings.Value.SourceDirectory;
            if (string.IsNullOrWhiteSpace(sourceDirectory) || !Directory.Exists(sourceDirectory))
            {
                throw new Exception("Source folder must be specified and exists!");
            }

            var destinationDirectory = _photoSorterSettings.Value.DestinationDirectory;
            if (string.IsNullOrWhiteSpace(destinationDirectory) || !Directory.Exists(destinationDirectory))
            {
                throw new Exception("Destination folder (root) must be specified and exists!");
            }

            var namePattern = _photoSorterSettings.Value.NamePattern;
            if (string.IsNullOrEmpty(namePattern))
            {
                throw new Exception("Name pattern must be specified!");
            }

            var useFileCreationDateIfNoExif = _photoSorterSettings.Value.UseFileCreationDateIfNoExif;

            return new PhotoSorterParameters(sourceDirectory, destinationDirectory, namePattern, useFileCreationDateIfNoExif);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Exiting with return code: {_exitCode}");

            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }
    }

}