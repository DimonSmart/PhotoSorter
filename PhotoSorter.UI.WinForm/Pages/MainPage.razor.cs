using Microsoft.AspNetCore.Components;
using PhotoSorterEngine;
using Radzen;
using Radzen.Blazor;

namespace PhotoSorter.UI.WinForm.Pages
{
    public partial class MainPage
    {
        private string sourceValue = string.Empty;
        private string destValue = string.Empty;

        private IEnumerable<string> _source;
        private IEnumerable<string> _dest;
        private FileReorderCalculationDescription _fileReorder;
        //public MainPage() 
        //{
        //    //MainPageState.Page = this;
        //}

        protected override void OnInitialized()
        {
            _source = new List<string>();
            _dest = new List<string>();
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
            _source = _fileReorder.FileMoveRequests.Select(r => r.SourceFileName).ToList();
            _dest = _fileReorder.FileMoveRequests.Select(r => r.DestinationFileName).ToList();
        }

        void LoadFiles(TreeExpandEventArgs args)
        {
            var directory = args.Value as string;

            args.Children.Data = Directory.EnumerateFileSystemEntries(directory);
            args.Children.Text = GetTextForNode;
            args.Children.HasChildren = (path) => {
                //return Directory.Exists((string)path);
                return false;
            };
            args.Children.Template = FileOrFolderTemplate;
        }

        RenderFragment<RadzenTreeItem> FileOrFolderTemplate = (context) => builder =>
        {
            string path = context.Value as string;
            bool isDirectory = Directory.Exists(path);

            builder.OpenComponent<RadzenIcon>(0);
            builder.AddAttribute(1, nameof(RadzenIcon.Icon), isDirectory ? "folder" : "insert_drive_file");
            builder.CloseComponent();
            builder.AddContent(3, context.Text);
        };

        private bool HasChildren(object path)
        {
            return Directory.Exists((string)path);
        }

        string GetTextForNode(object data)
        {
            return Path.GetFileName((string)data);
        }
    }
}
