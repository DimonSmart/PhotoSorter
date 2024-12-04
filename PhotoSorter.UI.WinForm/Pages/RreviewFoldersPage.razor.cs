using Microsoft.AspNetCore.Components;
using MudBlazor;
using PhotoSorter.UI.WinForm.Components;
using PhotoSorter.UI.WinForm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PhotoSorter.UI.WinForm.Pages
{
    public partial class RreviewFoldersPage
    {
        private MudTable<TreeItemBase>? sourceTable;
        private MyMudTable<TreeItemBase>? destTable;
        private int sourceSelectedRowNumber = -1;
        private int destSelectedRowNumber = -1;


        [Parameter]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public FolderTreeItem? SourceFolderItem { get; set; }

        [Parameter]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public FolderTreeItem? DestFolderItem { get; set; }

        [Parameter]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventCallback OnReturnClick { get; set; }

        [Parameter]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventCallback OnExecuteClick { get; set; }

        public string SourceFolderPath
        {
            get
            {
                if (SourceFolderItem.ParentFolder == null)
                    return string.Empty;

                FolderTreeItem? item = SourceFolderItem.ParentFolder;
                string result = SourceFolderItem.Name;
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
                if (DestFolderItem.ParentFolder == null)
                    return string.Empty;

                FolderTreeItem? item = DestFolderItem.ParentFolder;
                string result = DestFolderItem.Name;
                while (item != null)
                {
                    result = $"\\{item.Name}\\{result}";
                    item = item.ParentFolder;
                }

                return result;
            }
        }
        private void OnSourceReturnClick()
        {
            if (SourceFolderItem.ParentFolder != null)
                SourceFolderItem = SourceFolderItem.ParentFolder;
        }

        private void OnDestReturnClick()
        {
            if (DestFolderItem.ParentFolder != null)
                DestFolderItem = DestFolderItem.ParentFolder;
        }

        private string SourceSelectedRowClassFunc(TreeItemBase element, int rowNumber)
        {
            if (sourceSelectedRowNumber == rowNumber)
            {
                sourceSelectedRowNumber = -1;
                return string.Empty;
            }
            else if (sourceTable.SelectedItem != null && sourceTable.SelectedItem.Equals(element))
            {
                sourceSelectedRowNumber = rowNumber;
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
                return string.Empty;
            }
            else if (destTable.SelectedItem != null && destTable.SelectedItem.Equals(element))
            {
                destSelectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }

        private void SourceItemRowClickEvent(TableRowClickEventArgs<TreeItemBase> row)
        //private void OnSourceItemClick(TreeItemBase item)
        {
            var item = row.Item;
            //var treeItem = item as FolderTreeItem;
            if (item is FolderTreeItem folderItem)
            {
                SourceFolderItem = folderItem;
                return;
            }

            if (item is FileTreeItem fileItem)
            {
                FolderTreeItem destRootItem;
                if (DestFolderItem.ParentFolder == null)
                {
                    destRootItem = DestFolderItem;
                }
                else
                {
                    FolderTreeItem? treeItem = DestFolderItem.ParentFolder;
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

                    DestFolderItem = dest;
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
        {
            var item = row.Item;

            if (item is FolderTreeItem treeItem)
                DestFolderItem = treeItem;
        }

        private async void OnPageReturnClick()
        {
            await OnReturnClick.InvokeAsync();
        }

        private async void OnPageExecuteClick()
        {
            await OnExecuteClick.InvokeAsync();
        }
    }
}
