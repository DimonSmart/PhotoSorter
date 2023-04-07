using PhotoSorter.Commands;
using System.ComponentModel;

namespace PhotoSorter.Views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MainWindow _mainWindow;

        public event PropertyChangedEventHandler? PropertyChanged;
        public static SelectSourceFolderCommand SelectSourceFolderCommand { get; private set; }
        public static SelectDestinationFolderCommand SelectDestinationFolderCommand { get; private set; }

        public MainViewModel()
        {
            _mainWindow = (MainWindow)App.Current.MainWindow;
            SelectSourceFolderCommand = new SelectSourceFolderCommand(_mainWindow, this);
            SelectDestinationFolderCommand = new SelectDestinationFolderCommand(_mainWindow, this);
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
                    OnPropertyChanged(nameof(SourceFolderText));
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
                    OnPropertyChanged(nameof(DestinationFolderText));
                }
            }
        }
    }
}
