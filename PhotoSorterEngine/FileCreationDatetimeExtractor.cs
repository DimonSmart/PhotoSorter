using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using MetadataExtractor;
using MetadataExtractor.Formats.Avi;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.QuickTime;
using PhotoSorterEngine.Interfaces;
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

        readonly Dictionary<string, List<Func<string, Result<DateTime, Exception>>>> ext2Func =
            new Dictionary<string, List<Func<string, Result<DateTime, Exception>>>>(StringComparer.InvariantCultureIgnoreCase)
        {
                { MediaTypeExtensions.Jpg, new List<Func<string, Result<DateTime, Exception>>> { MetadataExtractorExtract } },
                { MediaTypeExtensions.Jpeg, new List<Func<string, Result<DateTime, Exception>>> { MetadataExtractorExtract } },
                { MediaTypeExtensions.Png, new List<Func<string, Result<DateTime, Exception>>> { MetadataExtractorExtract } },
                { MediaTypeExtensions.Heic, new List<Func<string, Result<DateTime, Exception>>> { MetadataExtractorExtract } },
                { MediaTypeExtensions.Dng, new List<Func<string, Result<DateTime, Exception>>> { MetadataExtractorExtract } },


                { MediaTypeExtensions.Avi, new List<Func<string, Result<DateTime, Exception>>> { FFmediaToolkitExtractorExtract } },
                { MediaTypeExtensions.Mp4, new List<Func<string, Result<DateTime, Exception>>> { FFmediaToolkitExtractorExtract } },
                { MediaTypeExtensions.Insp, new List<Func<string, Result<DateTime, Exception>>> { MetadataExtractorExtract, FFmediaToolkitExtractorExtract } },
        };

        private static List<(string dictionary, string tag, int id)> dateTimeTags = new()
        {
                new ("Exif IFD0", "Date/Time", ExifDirectoryBase.TagDateTime),
                new ("SubIFD", "Date/Time Original", ExifDirectoryBase.TagDateTimeOriginal),
                new ("QuickTime Movie Header", "Created", QuickTimeMetadataHeaderDirectory.TagCreationDate),
                new ("IPTC", "Date Created", IptcDirectory.TagDateCreated),
                new ("AVI", "Date/Time Original", AviDirectory.TagDateTimeOriginal)
        };

        public Result<DateTime, Exception> Extract(string fileName, bool useFileCreationDateIfNoExif)
        {
            var extension = Path.GetExtension(fileName);
            if (ext2Func.TryGetValue(extension, out var extractors))
            {
                foreach (var extractor in extractors)
                {
                    var result = extractor(fileName);
                    if (result.IsSuccess)
                    {
                        return result;
                    }
                }
            }

            if (useFileCreationDateIfNoExif)
            {
                return ExtractFileCreationDate(fileName);
            }

            return Result.Fail<DateTime, Exception>(new Exception("No way to extract the image taken date"));
        }

        private static Result<DateTime, Exception> MetadataExtractorExtract(string fileName)
        {
            return Extract(ImageMetadataReader.ReadMetadata(fileName));
        }

        private static Result<DateTime, Exception> FFmediaToolkitExtractorExtract(string fileName)
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
            catch (Exception exception)
            {
                return Result.Fail<DateTime, Exception>(exception);
            }

            return Result.Fail<DateTime, Exception>(new Exception("FFmediaToolkit extraction failed"));
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

            return Result.Fail<DateTime, Exception>(new Exception("MetadataExtractor extraction failed"));
        }
    }
}