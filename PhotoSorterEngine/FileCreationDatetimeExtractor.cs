using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using MetadataExtractor;
using MetadataExtractor.Formats.Avi;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.QuickTime;
using ResultMonad;
using System.IO.Abstractions;

namespace PhotoSorterEngine
{
    public class FileCreationDatetimeExtractor : IFileCreationDatetimeExtractor
    {
        static FileCreationDatetimeExtractor()
        {
            FFmpegLoader.FFmpegPath = @"./FFmpeg";
        }
        private IFileSystem _fileSystem { get; }


        public FileCreationDatetimeExtractor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private static List<(string dictionary, string tag, int id)> dateTimeTags = new()
        {
                new ("Exif IFD0", "Date/Time", ExifDirectoryBase.TagDateTime),
                new ("SubIFD", "Date/Time Original", ExifDirectoryBase.TagDateTimeOriginal),
                new ("QuickTime Movie Header", "Created", QuickTimeMetadataHeaderDirectory.TagCreationDate),
                new ("IPTC", "Date Created", IptcDirectory.TagDateCreated),
                new ("AVI", "Date/Time Original", AviDirectory.TagDateTimeOriginal)};

    
        public Result<DateTime, Exception> Extract(string fileName, bool useFileCreationDateIfNoExif)
        {
            var extension = Path.GetExtension(fileName);
            if (MediaTypeExtensions.IsVideo(extension))
            {
                return ExtractVideoCreationDateTime(fileName, useFileCreationDateIfNoExif);
            }

            return ExtractPhotoCreationDateTime(fileName, useFileCreationDateIfNoExif);
        }

        private Result<DateTime, Exception> ExtractPhotoCreationDateTime(string fileName, bool useFileCreationDateIfNoExif)
        {
            try
            {
                var result = Extract(ImageMetadataReader.ReadMetadata(fileName));
                if (result.IsSuccess)
                {
                    return result;
                }
            }
            catch (Exception exception)
            {
                if (!useFileCreationDateIfNoExif)
                {
                    return Result.Fail<DateTime, Exception>(exception);
                }
            }

            return ExtractCreationDateFallback(fileName, useFileCreationDateIfNoExif);
        }

        private Result<DateTime, Exception> ExtractVideoCreationDateTime(string fileName, bool useFileCreationDateIfNoExif)
        {
            try
            {
                using (var fileinfo = MediaFile.Open(fileName))
                {
                    if (fileinfo.Info.Metadata.Metadata.TryGetValue("creation_time", out var creationTimeAsString) && DateTime.TryParse(creationTimeAsString, out var creationTime))
                    {
                        return Result.Ok<DateTime, Exception>(creationTime);
                    }
                }
            }
            catch (Exception)
            {
            }

            return ExtractCreationDateFallback(fileName, useFileCreationDateIfNoExif);
        }

        private Result<DateTime, Exception> ExtractCreationDateFallback(string fileName, bool useFileCreationDateIfNoExif)
        {
            if (!useFileCreationDateIfNoExif)
            {
                return Result.Fail<DateTime, Exception>(new Exception("Cant extract creation date"));
            }
            return ExtractFileCreationDate(fileName);
        }

        private Result<DateTime, Exception> ExtractFileCreationDate(string fileName)
        {
            try
            {
                return Result.Ok<DateTime, Exception>(_fileSystem.File.GetCreationTime(fileName));

            }
            catch (Exception getFileCreationTimeException)
            {
                return Result.Fail<DateTime, Exception>(getFileCreationTimeException);
            }
        }

        private static Result<DateTime, Exception> Extract(IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            foreach (var dateTimeTag in dateTimeTags)
            {
                var dictionary = directories.FirstOrDefault(d => d.Name == dateTimeTag.dictionary);
                if (dictionary == null) continue;
                try
                {
                    if (dictionary.TryGetDateTime(dateTimeTag.id, out var dateTime))
                    {
                        return Result.Ok<DateTime, Exception>(dateTime);
                    }
                }
                catch (Exception)
                {
                }
            }

            return Result.Fail<DateTime, Exception>(new Exception("Cant extract creation date"));
        }
    }
}