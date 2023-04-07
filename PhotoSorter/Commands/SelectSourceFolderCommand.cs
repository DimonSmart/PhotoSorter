using PhotoSorter.Views;
using System.Windows.Forms;
using System.Windows.Input;

namespace PhotoSorter.Commands
{
    public class SelectSourceFolderCommand : RoutedCommand
    {
        private readonly MainWindow _mainWindow;
        private readonly MainViewModel _mainWiewModel;
        public SelectSourceFolderCommand(MainWindow mainWindow, MainViewModel mainWiewModel)
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
                // TODO: Check if it exists
                _mainWiewModel.SourceFolderText = dialog.SelectedPath;
            }
        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
