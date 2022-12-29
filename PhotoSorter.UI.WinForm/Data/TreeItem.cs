namespace PhotoSorter.UI.WinForm.Data
{
    public class TreeItem
    {
        public required string Name { get; set; }
    //    public required bool IsFile { get; set; }
   }

    public class FileTreeItem : TreeItem
    { 
    }

    public class FolderTreeItem : TreeItem
    {
        public Dictionary<string, TreeItem> Folders { get; } = new Dictionary<string, TreeItem>();
    }
}
