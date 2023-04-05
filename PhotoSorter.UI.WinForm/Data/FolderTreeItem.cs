namespace PhotoSorter.UI.WinForm.Data
{
    public class FolderTreeItem : TreeItemBase
    {
        public FolderTreeItem? ParentFolder { get; set; }
        public Dictionary<string, TreeItemBase> Folders { get; } = new Dictionary<string, TreeItemBase>();
    }
}
