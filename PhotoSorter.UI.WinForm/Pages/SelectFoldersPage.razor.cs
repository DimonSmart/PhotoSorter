using Microsoft.AspNetCore.Components;
using PhotoSorter.UI.WinForm.Data;
using Radzen;

namespace PhotoSorter.UI.WinForm.Pages
{
    public partial class SelectFoldersPage
    {
        private string sourceValue = string.Empty;
        private string destValue = string.Empty;

        [Parameter]
        public EventCallback<FoldersData> OnPreviewClick { get; set; }

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

        private async Task OnPreviewFoldersClick()
        {
            var result = MainPageState.ChsckFoldersExists(sourceValue, destValue);

            if (!string.IsNullOrEmpty(result))
            {
                _ = DialogService.Alert(result, "Error", new AlertOptions() { OkButtonText = "Yes" });
                return;
            }

            await OnPreviewClick.InvokeAsync(new FoldersData { Source = sourceValue, Dest = destValue });
        }
    }
}
