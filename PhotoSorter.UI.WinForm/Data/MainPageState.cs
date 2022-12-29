using PhotoSorterEngine;
using System.IO.Abstractions;
using System.Text;
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

        public FolderTreeItem ParseFolders(string rootPath, IEnumerable<string> folders)
        {
            var folderData = new FolderTreeItem
            {
                Name = rootPath
            };

            foreach(string path in folders)
            {
                var folderPath = path.Substring(rootPath.Length).TrimStart('\\');
                ParseFolder(folderData, folderPath, path);
            }

            return folderData;
        }

        private void ParseFolder(FolderTreeItem folderData, string path, string fullPath)
        {
            int i = path.IndexOf('\\');
            if (i > -1)
            {
                string nextPath = path.Substring(i + 1);

                string folderName = path.Substring(0, i);
                TreeItem? subFolderData = null;
                if (!folderData.Folders.TryGetValue(folderName, out subFolderData))
                {;
                    subFolderData = new FolderTreeItem
                    {
                        Name = folderName
                    };
                    folderData.Folders.Add(folderName, subFolderData);
                }

               // ParseFolder(subFolderData, nextPath, fullPath);
                return;
            }
            if (!string.IsNullOrEmpty(path))
                folderData.Folders.Add(path, new FileTreeItem { Name = path });
        }
    }
}
