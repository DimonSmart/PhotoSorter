using Microsoft.AspNetCore.Components;
using PhotoSorter.UI.WinForm.Data;
using PhotoSorterEngine;
using Radzen;
using Radzen.Blazor;

namespace PhotoSorter.UI.WinForm.Pages
{
    public partial class MainPage
    {
        private string sourceValue = string.Empty;
        private string destValue = string.Empty;

        //private IEnumerable<string> _source;
        //private IEnumerable<string> _dest;
        private TreeItem _source;
        private TreeItem _dest;
        private FileReorderCalculationDescription _fileReorder;

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
                return _dest.Folders.Values;
            }
        }
        //public MainPage() 
        //{
        //    //MainPageState.Page = this;
        //}

        protected override void OnInitialized()
        {
            _source = new TreeItem();
            _dest = new TreeItem();
        }

        private void OnClick(string buttonName)
        {
            var selectedFolder = MainPageState.SelectFolder();
            if (string.IsNullOrEmpty(selectedFolder))
                return;

            switch (buttonName)
            {
                case "source":
                    sourceValue = selectedFolder;
                    break;
                case "dest":
                    destValue = selectedFolder;
                    break;
            }
        }

        private void OnPreviewClick()
        {
            var result = MainPageState.ChsckFoldersExists(sourceValue, destValue);

            if (!string.IsNullOrEmpty(result))
            {
                DialogService.Alert(result, "Error", new AlertOptions() { OkButtonText = "Yes" });
                return;
            }

            _fileReorder = MainPageState.GetSourcePreview(sourceValue, destValue);
            _source = MainPageState.ParseFolders(sourceValue, _fileReorder.FileMoveRequests.Select(r => r.SourceFileName).ToList());//_fileReorder.FileMoveRequests.Select(r => r.SourceFileName).ToList();
            _dest = MainPageState.ParseFolders(destValue, _fileReorder.FileMoveRequests.Select(r => r.DestinationFileName).ToList());//_fileReorder.FileMoveRequests.Select(r => r.DestinationFileName).ToList();
        }

        void LoadFiles(TreeExpandEventArgs args)
        {
            args.Children.Data = ((TreeItem)args.Value).Folders.Values;
            args.Children.Text = GetTextForNode;
            args.Children.HasChildren = HasChildren;
            args.Children.Template = FileOrFolderTemplate;
        }

        RenderFragment<RadzenTreeItem> FileOrFolderTemplate = (context) => builder =>
        {
            bool isDirectory = !((TreeItem)context.Value).IsFile;

            builder.OpenComponent<RadzenIcon>(0);
            builder.AddAttribute(1, nameof(RadzenIcon.Icon), isDirectory ? "folder" : "insert_drive_file");
            builder.CloseComponent();
            builder.AddContent(3, context.Text);
        };

        private bool HasChildren(object path)
        {
            return ((TreeItem)path).Folders.Count() > 0;
        }

        string GetTextForNode(object data)
        {
            return ((TreeItem)data).Name;
        }
    }
}
