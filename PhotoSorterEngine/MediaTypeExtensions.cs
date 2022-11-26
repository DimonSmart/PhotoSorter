namespace PhotoSorterEngine
{
    public static class MediaTypeExtensions
    {
        [Flags]
        public enum MediaType
        {
            Images = 1,
            Video = 2,
            All = Images | Video
        }

        public const string Jpg = ".jpg";
        public const string Jpeg = ".jpeg";
        public const string Png = ".png";
        public const string Heic = ".heic";
        public const string Dng = ".dng";

        public const string Avi = ".avi";
        public const string Mp4 = ".mp4";
        public const string Insp = ".insp";

        public static readonly ICollection<string> Images = new List<string> { Jpg, Jpeg, Png, Heic, Dng };
        public static readonly ICollection<string> Video = new List<string> { Avi, Mp4, Insp };
        public static readonly ICollection<string> All = new List<string> { Jpg, Jpeg, Png, Heic, Dng, Avi, Mp4, Insp };

        public static ICollection<string> GetFileExtensions(MediaType mediaType)
        {
            return mediaType switch
            {
                MediaType.Images => Images,
                MediaType.Video => Video,
                MediaType.All => All,
                _ => throw new ArgumentException(nameof(mediaType)),
            };
        }

        internal static bool IsVideo(string extension)
        {
            return Video.Contains(extension, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}