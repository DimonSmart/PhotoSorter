using PhotoSorter.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace PhotoSorter.Views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MainWindow _mainWindow;

        public event PropertyChangedEventHandler? PropertyChanged;
        public static SourceFolderCommand SelectSourceFolderCommand { get; private set; }
        public static DestinationFolderCommand SelectDestinationFolderCommand { get; private set; }

        public MainViewModel()
        {
            _mainWindow = (MainWindow)App.Current.MainWindow;
            SelectSourceFolderCommand = new SourceFolderCommand(_mainWindow, this);
            SelectDestinationFolderCommand = new DestinationFolderCommand(_mainWindow, this);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _sourceFolderText;
        public string SourceFolderText
        {
            get { return _sourceFolderText; }
            set
            {
                if (value != _sourceFolderText)
                {
                    _sourceFolderText = value;
                    OnPropertyChanged("SourceFolderText");
                }
            }
        }

        private string _destinationFolderText;
        public string DestinationFolderText
        {
            get { return _destinationFolderText; }
            set
            {
                if (value != _destinationFolderText)
                {
                    _destinationFolderText = value;
                    OnPropertyChanged("DestinationFolderText");
                }
            }
        }
    }
}
