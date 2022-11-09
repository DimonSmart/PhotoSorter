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

        public static readonly ICollection<string> Images = new List<string> { ".jpg", ".jpeg", ".png", ".heic" };
        public static readonly ICollection<string> Video = new List<string> { ".avi", ".mp4" };
        public static readonly ICollection<string> All = new List<string> { ".jpg", ".jpeg", ".png", ".avi", ".mp4" };

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