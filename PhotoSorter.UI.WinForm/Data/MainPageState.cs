using MudBlazor;
using PhotoSorterEngine;
using System.Diagnostics;
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
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {

                        return dialog.SelectedPath;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
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

        public static FolderTreeItem ParseFolders(string rootPath, IEnumerable<FileMoveRequest> fileMoveRequests, Func<FileMoveRequest, string> selector)
        {
            var folderData = new FolderTreeItem
            {
                Name = rootPath
            };

            foreach (var request in fileMoveRequests.OrderBy(i => selector(i)))
            {
                ParseFile(folderData, rootPath, request, selector);
            }

            return folderData;
        }

        private static readonly char[] PathSeparators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        private static void ParseFile(FolderTreeItem folderTreeItem, string rootPath, FileMoveRequest fileMoveRequest, Func<FileMoveRequest, string> selector)
        {
            var fileName = selector(fileMoveRequest);
            var fileNameOnly = Path.GetFileName(fileName);
            var directoryFullNameOnly = Path.GetDirectoryName(fileName);
            Debug.Assert(directoryFullNameOnly != null);
            var relativePath = Path.GetRelativePath(rootPath, directoryFullNameOnly);
            if (relativePath != ".")
            {
                var relativePathSplitted = relativePath.Split(PathSeparators);
                foreach (var item in relativePathSplitted)
                {
                    if (folderTreeItem.Folders.TryGetValue(item, out var subFolderTreeItem))
                    {
                        if (subFolderTreeItem is FolderTreeItem fti && fti != null)
                        {
                            folderTreeItem = fti;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        var newFolderItem = new FolderTreeItem { Name = item };
                        folderTreeItem.Folders.Add(item, newFolderItem);
                        folderTreeItem = newFolderItem;
                    }
                }
            }
            folderTreeItem.Folders.Add(fileNameOnly, new FileTreeItem { Name = fileNameOnly, FileMoveRequest = fileMoveRequest });
        }

        internal FolderTreeItem ParseFolders(string sourceValue, ICollection<FileMoveRequest> fileMoveRequests, Func<object, object> value)
        {
            throw new NotImplementedException();
        }

        public static TreeItemData ParseFoldersData(string rootPath, IEnumerable<FileMoveRequest> fileMoveRequests, Func<FileMoveRequest, string> selector)
        {
            var treeItemData = new TreeItemData( rootPath, "" );

            foreach (var request in fileMoveRequests)
            {
                ParseFileData(treeItemData, rootPath, request, selector);
            }

            return treeItemData;
        }

        private static void ParseFileData(TreeItemData folderTreeItem, string rootPath, FileMoveRequest fileMoveRequest, Func<FileMoveRequest, string> selector)
        {
            var fileName = selector(fileMoveRequest);
            var fileNameOnly = Path.GetFileName(fileName);
            var directoryFullNameOnly = Path.GetDirectoryName(fileName);
            Debug.Assert(directoryFullNameOnly != null);
            var relativePath = Path.GetRelativePath(rootPath, directoryFullNameOnly);
            if (relativePath != ".")
            {
                var relativePathSplitted = relativePath.Split(PathSeparators);
                foreach (var item in relativePathSplitted)
                {
                    var treeItemData = folderTreeItem.TreeItems.FirstOrDefault(ti => ti.Text == item);
                    if (treeItemData != null)
                    {
                        folderTreeItem = treeItemData;
                    }
                    else
                    {
                        var newFolderItem = new TreeItemData( item, Icons.Material.Filled.Folder);
                        folderTreeItem.TreeItems.Add(newFolderItem);
                        folderTreeItem = newFolderItem;
                    }
                }
            }
            folderTreeItem.TreeItems.Add(new TreeItemData( fileNameOnly, Icons.Custom.FileFormats.FileDocument));
        }
    }
}
