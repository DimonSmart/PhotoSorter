using MudBlazor;
using System.ComponentModel;

namespace PhotoSorter.UI.WinForm.Components
{
    public class MyMudTable<T> : MudTable<T>
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public T? SelectAfterRender { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            SelectedItem = SelectAfterRender;
        }
    }
}
