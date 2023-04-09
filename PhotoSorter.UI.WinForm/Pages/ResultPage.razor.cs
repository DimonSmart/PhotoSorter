using Microsoft.AspNetCore.Components;

namespace PhotoSorter.UI.WinForm.Pages
{
    public partial class ResultPage
    {
        [Parameter]
        public EventCallback OnReturnClick { get; set; }

        private async Task OnPageReturnClick()
        {
            OnReturnClick.InvokeAsync();
        }
    }
}
