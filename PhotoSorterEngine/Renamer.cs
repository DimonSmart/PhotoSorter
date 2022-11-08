using Functional.Maybe;
using System.Globalization;

namespace PhotoSorterEngine
{
    public class Renamer : IRenamer
    {
        public Renamer()
        {
        }

        public string Rename(string sourceFileName, DateTime dateTime, string destinationFolder, string pattern)
        {
            //  var dateTime = _fileCreationDatetimeExtractor.Extract(sourceFileName);
            // if (!dateTime.HasValue) return Maybe<string>.Nothing;
            CultureInfo enUs = new CultureInfo("en-US");
            var substDict = new Dictionary<string, string>()
            {
                { "%YYYY%", dateTime.ToString("yyyy")},
                { "%MM%", dateTime.ToString("MM") },
                { "%MMM%", dateTime.ToString("MMM") },
                { "%MMM_ENG%", dateTime.ToString("MMM", enUs) },
                { "%MMMM_ENG%", dateTime.ToString("MMMM", enUs) },
                { "%MMMM_ENG_LOWER%", dateTime.ToString("MMMM", enUs).ToLower() },
                { "%MMMM%", dateTime.ToString("MMMM") },
                { "%d%", dateTime.ToString("d") },
                { "%dd%", dateTime.ToString("dd") },
            };

            foreach (var pair in substDict)
            {
                pattern = pattern.Replace(pair.Key, pair.Value, StringComparison.InvariantCultureIgnoreCase);
            }

            var fileName = Path.GetFileName(sourceFileName);
            var fullResultPath = Path.Combine(destinationFolder, pattern, fileName);
            return fullResultPath;
        }
    }
}