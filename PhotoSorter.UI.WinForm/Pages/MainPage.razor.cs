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
        private FolderTreeItem _source;
        private FolderTreeItem _dest;
        private FileReorderCalculationDescription _fileReorder;
        private RadzenTree? _destTree;

        private HashSet<TreeItemData> DestTreeItems { get; set; } = new HashSet<TreeItemData>();


        object selection;

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
        //public MainPage() 
        //{
        //    //MainPageState.Page = this;
        //}

        protected override void OnInitialized()
        {
            _source = new FolderTreeItem() { Name = "src_root" };
            _dest = new FolderTreeItem() { Name = "dst_root" };
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
            _source = MainPageState.ParseFolders(sourceValue, _fileReorder.FileMoveRequests, i => i.SourceFileName);
            _dest = MainPageState.ParseFolders(destValue, _fileReorder.FileMoveRequests, i => i.DestinationFileName);
            DestTreeItems = MainPageState.ParseFoldersData(destValue, _fileReorder.FileMoveRequests, i => i.DestinationFileName).TreeItems;
        }

        private void OnSelectClick()
        {
            //foreach(var d in _destTree.Data)
            //{

            //}

            if (DestTreeItems.Count() > 0)
            {
                var ti = DestTreeItems.First();

                while (ti.HasChild)
                {
                    ti.IsExpanded = true;
                    ti = ti.TreeItems.First();
                }
            }
        }

        void LoadFiles(TreeExpandEventArgs args)
        {
            IEnumerable<TreeItemBase> childs = Enumerable.Empty<TreeItemBase>();
            if (args.Value is FolderTreeItem fti)
            {
                childs = fti.Folders.Values;
            }

            args.Children.Data = childs;
            args.Children.Text = GetTextForNode;
            args.Children.HasChildren = HasChildren;
            args.Children.Template = FileOrFolderTemplate;
        }

        RenderFragment<RadzenTreeItem> FileOrFolderTemplate = (context) => builder =>
        {
            bool isDirectory = context.Value is FolderTreeItem;

            builder.OpenComponent<RadzenIcon>(0);
            builder.AddAttribute(1, nameof(RadzenIcon.Icon), isDirectory ? "folder" : "insert_drive_file");
            builder.CloseComponent();
            builder.AddContent(3, context.Text);
        };

        private bool HasChildren(object path)
        {
            return path is FolderTreeItem folder && folder.Folders.Any();
        }

        string GetTextForNode(object data)
        {
            return ((TreeItemBase)data).Name;
        }

        bool Expanded(object data)
        {
            return (data as TreeItemBase).Expanded;
            //var treeNode = data as FolderTreeItem;
            //if (treeNode != null)
            //{
            //    return treeNode.Folders.Count() > 0 && treeNode.Expanded;
            //}
            //return false;
        }
    }
}
