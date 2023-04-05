using Microsoft.AspNetCore.Components;
using MudBlazor;
using PhotoSorter.UI.WinForm.Components;
using PhotoSorter.UI.WinForm.Data;
using PhotoSorterEngine;
using Radzen;
using Radzen.Blazor;
using System.IO.Abstractions;

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

        MudListItem sourceSelectedItem;
        object sourceSelectedValue;

        MudListItem destSelectedItem;
        object destSelectedValue;

        private MudList SouceList;

        private MudTable<TreeItemBase>? sourceTable;
        private MyMudTable<TreeItemBase>? destTable;
        private int sourceSelectedRowNumber = -1;
        private int destSelectedRowNumber = -1;


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

        public string SourceFolderPath
        {
            get
            {
                if (_source.ParentFolder == null)
                    return string.Empty;

                FolderTreeItem? item = _source.ParentFolder;
                string result = _source.Name;
                while (item != null)
                {
                    result = $"\\{item.Name}\\{result}";
                    item = item.ParentFolder;
                }

                return result;
            }
        }

        public string DestFolderPath
        {
            get
            {
                if (_dest.ParentFolder == null)
                    return string.Empty;

                FolderTreeItem? item = _dest.ParentFolder;
                string result = _dest.Name;
                while (item != null)
                {
                    result = $"\\{item.Name}\\{result}";
                    item = item.ParentFolder;
                }

                return result;
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

        private void SourceItemRowClickEvent(TableRowClickEventArgs<TreeItemBase> row)
        //private void OnSourceItemClick(TreeItemBase item)
        {
            var item = row.Item;
            //var treeItem = item as FolderTreeItem;
            if (item is FolderTreeItem folderItem)
            {
                _source = folderItem;
                return;
            }

            if (item is FileTreeItem fileItem)
            {
                FolderTreeItem destRootItem;
                if (_dest.ParentFolder == null)
                {
                    destRootItem = _dest;
                }
                else
                {
                    FolderTreeItem? treeItem = _dest.ParentFolder;
                    while (treeItem.ParentFolder != null)
                    {
                        treeItem = treeItem.ParentFolder;
                    }
                    destRootItem = treeItem;
                }

                var dest = FindCurrentFileName(item.Name, destRootItem);

                if (dest != null)
                {
                    var selectedItem = dest.Folders.FirstOrDefault(i => i.Value.Name == item.Name);
                    destTable.SelectAfterRender = selectedItem.Value;

                    _dest = dest;
                    //StateHasChanged();
                    //destTable.SelectedItem = dest.Folders.Values.FirstOrDefault(i => i.Name == item.Name);
                    //if (selectedItem != null)
                    //    destTable.SelectedItem = selectedItem;                    
                }
            }
        }

        private FolderTreeItem? FindCurrentFileName(string fileName, FolderTreeItem folderTreeItem)
        {
            foreach (var item in folderTreeItem.Folders.Values)
            {
                if (item is FileTreeItem fileItem) 
                {
                    if (item.Name == fileName)
                        return folderTreeItem; 
                }

                if (item is FolderTreeItem folderItem)
                {
                    var ftItem = FindCurrentFileName(fileName, folderItem);
                    if (ftItem != null)
                        return ftItem;
                }
            }
            return null;
        }

        private void DestItemRowClickEvent(TableRowClickEventArgs<TreeItemBase> row)
        //private void OnDestItemClick(TreeItemBase item)
        {
            //var treeItem = item as FolderTreeItem;
            var item = row.Item;

            if (item is FolderTreeItem treeItem)
                _dest = treeItem;
        }

        private string SourceSelectedRowClassFunc(TreeItemBase element, int rowNumber)
        {
            if (sourceSelectedRowNumber == rowNumber)
            {
                sourceSelectedRowNumber = -1;
                //clickedEvents.Add("Selected Row: None");
                return string.Empty;
            }
            else if (sourceTable.SelectedItem != null && sourceTable.SelectedItem.Equals(element))
            {
                sourceSelectedRowNumber = rowNumber;
                //clickedEvents.Add($"Selected Row: {rowNumber}");
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

        private string DestSelectedRowClassFunc(TreeItemBase element, int rowNumber)
        {
            if (destSelectedRowNumber == rowNumber)
            {
                destSelectedRowNumber = -1;
                //clickedEvents.Add("Selected Row: None");
                return string.Empty;
            }
            else if (destTable.SelectedItem != null && destTable.SelectedItem.Equals(element))
            {
                destSelectedRowNumber = rowNumber;
                //clickedEvents.Add($"Selected Row: {rowNumber}");
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

        private void OnSourceReturnClick()
        {
            if (_source.ParentFolder != null)
                _source = _source.ParentFolder;
        }

        private void OnDestReturnClick()
        {
            if (_dest.ParentFolder != null)
                _dest = _dest.ParentFolder;
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
            _source = MainPageState.ParseFolders(sourceValue, _fileReorder.FileReorderRequests, i => i.SourceFileName);
            _dest = MainPageState.ParseFolders(destValue, _fileReorder.FileReorderRequests, i => i.DestinationFileName);
            DestTreeItems = MainPageState.ParseFoldersData(destValue, _fileReorder.FileReorderRequests, i => i.DestinationFileName).TreeItems;
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
