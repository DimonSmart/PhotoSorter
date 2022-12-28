namespace PhotoSorter.UI.WinForm.Data
{
    public class TreeItem
    {
        public string Name { get; set; }
        public bool IsFile { get; set; }
        public Dictionary<string, TreeItem>  Folders { get; } = new Dictionary<string, TreeItem>();
    }
}
