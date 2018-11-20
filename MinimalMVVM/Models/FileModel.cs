namespace MinimalMVVM.Models
{
    public class FileModel : ObservableObject
    {
        string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value;
                RaisePropertyChangedEvent("FilePath");
                System.Windows.MessageBox.Show("File path changed");
            }
        }
    }
}
