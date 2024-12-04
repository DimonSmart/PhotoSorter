using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace PhotoSorter.UI.WinForm.Pages
{
    public partial class ResultPage
    {
        [Parameter]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EventCallback OnReturnClick { get; set; }

        private async Task OnPageReturnClick()
        {
            await OnReturnClick.InvokeAsync();
        }
    }
}
