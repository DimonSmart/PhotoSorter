using MudBlazor;

namespace PhotoSorter.UI.WinForm.Components
{
    public class MyMudTable<T> : MudTable<T>
    {
        public T SelectAfterRender { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            SelectedItem = SelectAfterRender;
        }
    }
}
