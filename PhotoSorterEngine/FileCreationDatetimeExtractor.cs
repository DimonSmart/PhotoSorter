using Functional.Maybe;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using MetadataExtractor.Formats.QuickTime;

namespace PhotoSorterEngine
{
    public class FileCreationDatetimeExtractor : IFileCreationDatetimeExtractor
    {
        private static List<(string dictionary, string tag, int id)> dateTimeTags = new()
        {
                new ("Exif IFD0", "Date/Time", ExifDirectoryBase.TagDateTime),
                new ("SubIFD", "Date/Time Original", ExifDirectoryBase.TagDateTimeOriginal),
                new ("QuickTime Movie Header", "Created", QuickTimeMetadataHeaderDirectory.TagCreationDate),
                new ("IPTC", "Date Created", IptcDirectory.TagDateCreated)};
        /*
Exif IFD0 - Date/Time
Exif SubIFD - Date/Time Original
Exif SubIFD - Date/Time Digitized
ICC Profile - Profile Date/Time
IPTC - Date Created
File - File Modified Date
GPS - GPS Date Stamp
QuickTime Movie Header - Created
QuickTime Movie Header - Modified
QuickTime Track Header - Created
QuickTime Track Header - Modified
*/
        public Maybe<DateTime> Extract(string fileName)
        {
            return Extract(ImageMetadataReader.ReadMetadata(fileName));
        }

        public Maybe<DateTime> Extract(Stream fileStream)
        {
            return Extract(ImageMetadataReader.ReadMetadata(fileStream));
        }

        private static Maybe<DateTime> Extract(IReadOnlyList<MetadataExtractor.Directory> directories)
        {
            foreach (var dateTimeTag in dateTimeTags)
            {
                var dictionary = directories.FirstOrDefault(d => d.Name == dateTimeTag.dictionary);
                if (dictionary == null) continue;
                try
                {
                    if (dictionary.TryGetDateTime(dateTimeTag.id, out var dateTime))
                    {
                        return dateTime.ToMaybe();
                    }
                }
                catch (Exception)
                {
                }
            }
            return Maybe<DateTime>.Nothing;
        }
    }
}