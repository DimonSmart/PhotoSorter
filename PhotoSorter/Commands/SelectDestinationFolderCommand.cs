using PhotoSorter.Views;
using System.Windows.Forms;
using System.Windows.Input;

namespace PhotoSorter.Commands
{
    public class SelectDestinationFolderCommand : RoutedCommand
    {
        private readonly MainWindow _mainWindow;
        private readonly MainViewModel _mainWiewModel;
        public SelectDestinationFolderCommand(MainWindow mainWindow, MainViewModel mainWiewModel)
        {
            _mainWindow = mainWindow;
            _mainWiewModel = mainWiewModel;
            _mainWindow.CommandBindings.Add(new CommandBinding(this, ExecutedCustomCommand, CanExecuteCustomCommand));
        }

        private void ExecutedCustomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                _mainWiewModel.DestinationFolderText = dialog.SelectedPath;
            }
        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
