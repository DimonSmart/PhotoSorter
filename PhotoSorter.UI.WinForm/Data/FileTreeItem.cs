using PhotoSorterEngine;

namespace PhotoSorter.UI.WinForm.Data
{
    public class FileTreeItem : TreeItemBase
    {
        public required FileMoveRequest FileMoveRequest { get; set; }
    }
}
