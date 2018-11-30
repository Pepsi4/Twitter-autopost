namespace MinimalMVVM
{
    public class FileModel
    {
        string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
            }
        }

        string _allUsersFilePath = "C://";
        public string AllUsersFilePath
        {
            get { return _allUsersFilePath; }
            set
            {
                _allUsersFilePath = value;
            }
        }

        string _whiteUsersFilePath = "C://";
        public string WhiteUsersFilePath
        {
            get { return _whiteUsersFilePath; }
            set
            {
                _whiteUsersFilePath = value;
            }
        }
    }
}
