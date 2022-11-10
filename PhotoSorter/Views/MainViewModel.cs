using System.ComponentModel;

namespace PhotoSorter.Views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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
                    _sourceFolderText = value;
                    OnPropertyChanged("DestinationFolderText");
                }
            }
        }
    }
}
