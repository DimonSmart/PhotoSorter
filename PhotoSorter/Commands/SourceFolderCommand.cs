using PhotoSorter.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace PhotoSorter.Commands
{
    public class SourceFolderCommand: RoutedCommand
    {
        private MainWindow _mainWindow;
        private MainViewModel _mainWiewModel;
        public SourceFolderCommand(MainWindow mainWindow, MainViewModel mainWiewModel)
        {
            _mainWindow = mainWindow;
            _mainWiewModel = mainWiewModel;
            _mainWindow.CommandBindings.Add(new CommandBinding(this, ExecutedCustomCommand, CanExecuteCustomCommand));
        }

        private void ExecutedCustomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {

                    _mainWiewModel.SourceFolderText = dialog.SelectedPath;
                }
            }
        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

    }
}
