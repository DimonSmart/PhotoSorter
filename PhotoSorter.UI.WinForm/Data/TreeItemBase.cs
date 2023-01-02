namespace PhotoSorter.UI.WinForm.Data
{
    public abstract class TreeItemBase
    {
        public bool Expanded { get; set; } = true;
        public required string Name { get; set; }
    }
}
