using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using PhotoSorter.UI.WinForm.Data;
using PhotoSorter.UI.WinForm.Pages;
using System.Net.NetworkInformation;

namespace PhotoSorter.UI.WinForm
{
    public partial class MainForm : Form
    {
        private readonly MainPageState _mainPageState = new();

        public MainForm()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWindowsFormsBlazorWebView();
            serviceCollection.AddSingleton<MainPageState>(_mainPageState);

            InitializeComponent();

            //var services = new ServiceCollection();
            //services.AddWindowsFormsBlazorWebView();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = serviceCollection.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<MainPage>("#app");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}