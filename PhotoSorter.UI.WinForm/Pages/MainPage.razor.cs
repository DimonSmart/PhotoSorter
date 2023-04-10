using PhotoSorter.UI.WinForm.Data;

namespace PhotoSorter.UI.WinForm.Pages
{
    public partial class MainPage
    {
        private PageStateEnum PageState;
        private bool IndicatoVisible;

        private FolderTreeItem _source;
        private FolderTreeItem _dest;

        public IEnumerable<object> Source
        {
            get
            {
                return _source.Folders.Values;
            }
        }

        public IEnumerable<object> Dest
        {
            get
            {
                return _dest.Folders.Values.ToHashSet();
            }
        }

        public HashSet<TreeItemBase> DestItems
        {
            get { return _dest.Folders.Values.ToHashSet(); }
        }

        private void PreviewReturnClick()
        {
            PageState = PageStateEnum.SelectFolders;
        }

        private void ResultReturnClick()
        {
            PageState = PageStateEnum.SelectFolders;
        }

        private void ExecuteClick()
        {
            PageState = PageStateEnum.ShowResult;
        }

        public async Task ShowPreviewClick(FoldersData folderData)
        {
            IndicatoVisible = true;
            try
            {
                await Task.Run(() =>
                {
                    //Task.Delay(5000);
                    var fileReorder = MainPageState.GetSourcePreview(folderData.Source, folderData.Dest);
                    _source = MainPageState.ParseFolders(folderData.Source, fileReorder.FileReorderRequests, i => i.SourceFileName);
                    _dest = MainPageState.ParseFolders(folderData.Dest, fileReorder.FileReorderRequests, i => i.DestinationFileName);
                    PageState = PageStateEnum.PreviewFolders;
                });
            }
            finally
            {
                IndicatoVisible = false;
            }
        }
    }
}
