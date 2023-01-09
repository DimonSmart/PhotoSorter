using PhotoSorterEngine;

namespace PhotoSorter.UI.WinForm.Data
{
    public class FileTreeItem : TreeItemBase
    {
        public required FileReorderRequest FileReorderRequest { get; set; }
    }
}
