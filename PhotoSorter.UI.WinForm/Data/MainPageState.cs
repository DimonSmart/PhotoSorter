using PhotoSorter.UI.WinForm.Pages;
using PhotoSorter.Views;
using PhotoSorterEngine;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PhotoSorterEngine.MediaTypeExtensions;

namespace PhotoSorter.UI.WinForm.Data
{
    public class MainPageState
    {
        //public MainPage Page { get; set; }

        public string SelectFolder()
        {
            try
            {


                using (var dialog = new FolderBrowserDialog())
                {
                    DialogResult result = dialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {

                        return dialog.SelectedPath;
                    }
                }
                return string.Empty;
            } 
            catch(Exception ex) 
            {
                return string.Empty;
            }
        }

        public string ChsckFoldersExists(string source, string dest)
        {
            StringBuilder sb = new StringBuilder();

            if (!Directory.Exists(source))
            {
                sb.Append($" Source folder '{source}' not exists");
            }

            if (!Directory.Exists(dest))
            {
                sb.Append($" Dest folder '{dest}' not exists");
            }

            return sb.ToString();
        }

        public FileReorderCalculationDescription GetSourcePreview(string source, string dest)
        {
            var fileCreationDatetimeExtractor = new FileCreationDatetimeExtractor(new FileSystem());
            var renamer = new Renamer();
            var fileSorter = new FileReorderCalculator(renamer, fileCreationDatetimeExtractor);
            var fileEnumerator = new FileEnumerator();
            var sourceFiles = fileEnumerator.EnumerateFiles(source, MediaType.All);
            var result = fileSorter.Calculate(sourceFiles,
                new SortParameters(
                    dest,
                    @"%YYYY%\%Comment%%YYYY%-%MM%-%DD%%Comment%",
                    UseFileCreationDateIfNoExif: false));

            return result;
        }
    }
}
