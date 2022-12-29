namespace PhotoSorter.UI.WinForm.Data
{
    public class FolderTreeItem : TreeItemBase
    {
        public Dictionary<string, TreeItemBase> Folders { get; } = new Dictionary<string, TreeItemBase>();
    }
}
