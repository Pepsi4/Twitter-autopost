using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MinimalMVVM.Models;
using System.IO;
using System.Linq;
using TweetSharp;
using MinimalMVVM.ViewModels;
using System.Diagnostics;

namespace MinimalMVVM
{
    public class WindowViewModel : ObservableObject
    {
        #region constructors
        public WindowViewModel() { ChangeList(); }

        public WindowViewModel(IWindowsController windowsController)
        {
            if (windowsController == null) throw new System.ArgumentNullException(nameof(windowsController));
            _windowsController = windowsController;

            ChangeList();
        }
        #endregion

        #region fields
        IWindowsController _windowsController;

        private readonly FileModel fileModel = new FileModel();
        private readonly TwitterUserModel twitterUserModel = new TwitterUserModel();

        private ObservableCollection<string> _history = new ObservableCollection<string>();

        private ObservableCollection<string> _allUsersList = new ObservableCollection<string>();
        private ObservableCollection<string> _whiteListHistory = new ObservableCollection<string>();

        public string AllUsersFilePath
        {
            get { return fileModel.AllUsersFilePath; }
            set
            {
                fileModel.AllUsersFilePath = value;
                RaisePropertyChangedEvent(nameof(AllUsersFilePath));
            }
        }

        public string WhiteUsersFilePath
        {
            get { return fileModel.WhiteUsersFilePath; }
            set
            {
                fileModel.WhiteUsersFilePath = value;
                RaisePropertyChangedEvent(nameof(WhiteUsersFilePath));
            }
        }

        private bool _isAllusersListSelected = true;
        public bool IsAllUsersListSelected
        {
            get { return _isAllusersListSelected; }
            set
            {
                if (_isAllusersListSelected != value)
                {
                    _isAllusersListSelected = value;
                    ChangeList();
                }
            }
        }

        private string _someText;
        public string SomeText
        {
            get
            {
                return _someText;
            }
            set
            {
                _someText = value;
                RaisePropertyChangedEvent(nameof(SomeText));
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                RaisePropertyChangedEvent(nameof(FilePath));
            }
        }

        private int? _selectedLineIndex;
        public int? SelectedLineIndex
        {
            get { return _selectedLineIndex; }

            set
            {
                _selectedLineIndex = value;
                RaisePropertyChangedEvent(nameof(SelectedLineIndex));
            }
        }

        public IEnumerable<string> History
        {
            get { return _history; }

            set
            {
                _history = (ObservableCollection<string>)value;
                RaisePropertyChangedEvent(nameof(History));
            }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChangedEvent(nameof(UserName));
            }
        }

        private long _userId;
        public long UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                RaisePropertyChangedEvent(nameof(UserId));
            }
        }

        private bool _isHttp;
        public bool IsHttp
        {
            get { return _isHttp; }
            set
            {
                _isHttp = value;
                RaisePropertyChangedEvent(nameof(IsHttp));
            }
        }

        private static string ConsumerKey = "";
        private static string ConsumerSecret = "";
        private static string _accessToken = "";
        private static string _accessTokenSecret = "";

        private static TwitterService service = new TwitterService(ConsumerKey, ConsumerSecret, _accessToken, _accessTokenSecret);
        #endregion

        #region commands

        public ICommand ChangeListCommand
        {
            get { return new DelegateCommand(ChangeList, true); }
        }

        //ICommand _convertTextCommand;
        public ICommand ConvertTextCommand
        {
            //get
            //{
            //    return _convertTextCommand ?? (_convertTextCommand = new DelegateCommand(param => ConvertText(param), CanExecuteAttachmentChecked()));
            //}
            get { return new DelegateCommand(ConvertText, true); }
        }

        private bool CanExecuteAttachmentChecked()
        {
            return true;
        }

        private ICommand _createFileCommand;

        public ICommand CreateFileCommand
        {
            get
            {
                return _createFileCommand ?? (_createFileCommand = new DelegateCommand(param => CreateFile(param), CanExecuteAttachmentChecked()));
            }
        }

        //public ICommand AddTextFromFileCommand
        //{
        //    get { return new DelegateCommand(AddTextFromFile, true); }
        //}

        public ICommand DeleteLineFromTextCommand
        {
            get { return new DelegateCommand(DeleteLineFromText, true); }
        }

        ICommand _getFilePath;
        public ICommand GetFilePathCommand
        {
            get
            {
                return _getFilePath ?? (_getFilePath = new DelegateCommand(param => OpenFile(param, true, null), CanExecuteAttachmentChecked()));
            }
        }

        #endregion

        #region command-methods

        private void CreateFile(object obj)
        {
            string path = "";

            switch (obj.ToString())
            {
                case "AllUsers":
                    path = "AllUsers.txt";
                    break;

                case "WhiteUsers":
                    path = "WhiteUsersList.txt";
                    break;
            }

            if (File.Exists(path))
            {
                _windowsController.ShowMessage("Файл уже существует.");
                OpenFile(obj, false, path);
            }
            else
            {
                FileStream file = File.Create(path);
                _windowsController.ShowMessage("Файл создан.");
            }
        }

        private void OpenFile(object buttonName, bool isItNewFile, string filePath)
        {
            if (isItNewFile)
            {
                filePath = _windowsController.ShowFileDialog();
            }

            ClearHistory();

            if (filePath != null && filePath != "")
            {
                switch (buttonName.ToString())
                {
                    case "AllUsers":
                        AllUsersFilePath = filePath;
                        _allUsersList.Clear();
                        break;

                    case "WhiteUsers":
                        WhiteUsersFilePath = filePath;
                        _whiteListHistory.Clear();
                        break;
                }

                fileModel.FilePath = filePath;
                GetTextFromFile(buttonName.ToString());
                ChangeList();


                ////ClearHistory();
                //Debug.Write("File path: " + filePath);

                //switch (buttonName.ToString())
                //{
                //    case "AllUsers":
                //        AllUsersFilePath = filePath;
                //        _allUsersList.Clear();
                //        break;

                //    case "WhiteUsers":
                //        WhiteUsersFilePath = filePath;
                //        _whiteListHistory.Clear();
                //        break;
                //}
                ////ChangeCurrentFile();
                ////fileModel.FilePath = filePath;
                //fileModel.FilePath = FilePath;
                //GetTextFromFile(buttonName.ToString());
            }
            else
            {
                _windowsController.ShowMessage("Ошибка файла");
            }
        }

        private void ChangeList()
        {
            ChangeCurrentFile();

            if (_isAllusersListSelected == false)
            {
                for (int i = 0; i < _history.Count; i++)
                {
                    if (_allUsersList.Contains(_history[i]) == false)
                        _allUsersList.Add(_history[i]);
                }

                _history.Clear();

                for (int i = 0; i < _whiteListHistory.Count; i++)
                {
                    if (_history.Contains(_whiteListHistory[i]) == false)
                        _history.Add(_whiteListHistory[i]);
                }
            }
            else
            {
                for (int i = 0; i < _history.Count; i++)
                {
                    if (_whiteListHistory.Contains(_history[i]) == false)
                        _whiteListHistory.Add(_history[i]);
                }

                _history.Clear();

                for (int i = 0; i < _allUsersList.Count; i++)
                {
                    if (_history.Contains(_allUsersList[i]) == false)
                        _history.Add(_allUsersList[i]);
                }
            }
        }


        private void ConvertText()
        {
            if (IsHttp == true)
            {
                AddHttpText();
            }
            else
            {
                AddIdText();
            }
        }

        private void DeleteLineFromText()
        {
            try
            {
                List<string> linesList = File.ReadAllLines(fileModel.FilePath).ToList();
                linesList.RemoveAt((int)_selectedLineIndex);
                File.WriteAllLines(fileModel.FilePath, _history);
            }
            catch (System.ArgumentNullException) { }
            catch (System.ArgumentOutOfRangeException) { }



            if (_isAllusersListSelected) DeleteLineFromList(_allUsersList);
            else DeleteLineFromList(_whiteListHistory);

            DeleteLineHistory();

            SaveChangesInFile();
        }

        //private void SelectFile()
        //{
        //    string filePath = _windowsController.ShowFileDialog();
        //    if (filePath != null)
        //    {
        //        fileModel.FilePath = filePath;
        //    }
        //}

        //private void AddTextFromFile()
        //{
        //    ClearHistory();
        //    SelectFile();

        //    GetTextFromFile();
        //    SaveChangesInFile();
        //}


        #endregion

        #region helpers

        // File section



        private void ChangeCurrentFile()
        {
            if (IsAllUsersListSelected) fileModel.FilePath = fileModel.AllUsersFilePath;
            else fileModel.FilePath = fileModel.WhiteUsersFilePath;
        }

        private void GetTextFromFile(string listName)
        {
            if (fileModel.FilePath != null && fileModel.FilePath != "")
            {
                var fileStream = new FileStream(fileModel.FilePath, FileMode.Open);
                var streamReader = new StreamReader(fileStream, System.Text.Encoding.Default);

                string data = null;
                while ((data = streamReader.ReadLine()) != null)
                {
                    switch (listName)
                    {
                        case "AllUsers":
                            _allUsersList.Add(data);
                            break;

                        case "WhiteUsers":
                            _whiteListHistory.Add(data);
                            break;
                    }

                    //_history.Add(data);
                }

                Debug.WriteLine("_allUsersList length: " + _allUsersList.Count);
                Debug.WriteLine("_whiteListHistory length: " + _whiteListHistory.Count);
                Debug.WriteLine("History length: " + _history.Count);

                fileStream.Close();
                streamReader.Close();
            }
            else { _windowsController.ShowMessage("Ошибка файла"); }
        }

        public void SaveChangesInFile()
        {
            try
            {
                if (fileModel.FilePath != null)
                {
                    File.WriteAllLines(fileModel.FilePath, _history.ToList<string>());
                }
            }
            catch (System.UnauthorizedAccessException ex) { _windowsController.ShowMessage(ex.Message); }
            catch (System.Exception ex) { _windowsController.ShowMessage(ex.Message); }
        }



        //TweetSharp section

        private void SetUserName()
        {
            twitterUserModel.UserName = _someText.Substring(_someText.IndexOf("/", 15) + 1);  //converting http to user's name. //magic numbers in mvvm??
        }

        private void ConvertHttpToId()
        {
            var user = service.GetUserProfileFor(new GetUserProfileForOptions
            {
                ScreenName = twitterUserModel.UserName
            });

            twitterUserModel.UserId = user.Id;
        }

        //History section

        private void AddIdText()
        {
            AddToHistory(SomeText);
            ClearInput();
            SaveChangesInFile();
        }

        private void AddHttpText()
        {
            if (string.IsNullOrWhiteSpace(SomeText)) return;    // Checks if we have something in the field.
            SetUserName();                                      // Converts http to user`s name.
            ConvertHttpToId();                                  // Converts user's name to his id.
            AddToHistory(twitterUserModel.UserId.ToString());   // Adds to history.
            ClearInput();                                       // Deletes text in the input field
            SaveChangesInFile();                                // Saves history to the file.
        }

        private void ClearInput()
        {
            SomeText = string.Empty;
        }

        private void ClearHistory()
        {
            _history.Clear();
        }

        private void AddToHistory(string item)
        {
            if (!_history.Contains(item))
                _history.Add(item);
        }

        private void DeleteLineHistory()
        {
            if (_selectedLineIndex != null && _selectedLineIndex >= 0)
            {
                _history.RemoveAt((int)_selectedLineIndex);
            }
        }

        private void DeleteLineFromList(ObservableCollection<string> list)
        {
            if (_selectedLineIndex != null && _selectedLineIndex >= 0)
            {
                list.RemoveAt((int)_selectedLineIndex);
            }
        }
        #endregion
    }
}
