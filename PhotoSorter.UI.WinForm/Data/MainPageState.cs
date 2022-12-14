using PhotoSorter.UI.WinForm.Pages;
using PhotoSorter.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSorter.UI.WinForm.Data
{
    public class MainPageState
    {
        //public MainPage Page { get; set; }

        public string SelectFolder()
        {
            try
            {


                using (var dialog = new FolderBrowserDialog())
                {
                    DialogResult result = dialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {

                        return dialog.SelectedPath;
                    }
                }
                return string.Empty;
            } 
            catch(Exception ex) 
            {
                return string.Empty;
            }
        }

        public string ChsckFoldersExists(string source, string dest)
        {
            StringBuilder sb = new StringBuilder();

            if (!Directory.Exists(source))
            {
                sb.Append($" Source folder '{source}' not exists");
            }

            if (!Directory.Exists(dest))
            {
                sb.Append($" Dest folder '{dest}' not exists");
            }

            return sb.ToString();
        }
    }
}
