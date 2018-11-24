using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MinimalMVVM.Models;
using System.IO;
using System.Linq;
using TweetSharp;
using MinimalMVVM.ViewModels;

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
                System.Windows.MessageBox.Show("changed");
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

        //public ICommand ChangeListCommand
        //{
        //    get { return new DelegateCommand(ChangeList); }
        //}

        public ICommand ConvertTextCommand
        {
            get { return new DelegateCommand(ConvertText); }
        }

        public ICommand AddTextFromFileCommand
        {
            get { return new DelegateCommand(AddTextFromFile); }
        }

        public ICommand DeleteLineFromTextCommand
        {
            get { return new DelegateCommand(DeleteLineFromText); }
        }

        #endregion

        #region command-methods

        private void ChangeList()
        {
            if (_isAllusersListSelected)
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

            DeleteLineHistory();
            SaveChangesInFile();
        }

        private void AddTextFromFile()
        {
            ClearHistory();
            SelectFile();

            GetTextFromFile();
            SaveChangesInFile();
        }
        #endregion

        #region helpers

        // File section

        private void GetTextFromFile()
        {
            if (fileModel.FilePath != null)
            {
                var fileStream = new FileStream(fileModel.FilePath, FileMode.Open);
                var streamReader = new StreamReader(fileStream, System.Text.Encoding.Default);

                string data = null;
                while ((data = streamReader.ReadLine()) != null)
                {
                    _history.Add(data);
                }

                fileStream.Close();
                streamReader.Close();
            }
        }

        public void SaveChangesInFile()
        {
            if (fileModel.FilePath != null)
            {
                File.WriteAllLines(fileModel.FilePath, _history.ToList<string>());
            }
        }

        private void SelectFile()
        {
            string filePath = _windowsController.ShowFileDialog();
            if (filePath != null)
            {
                fileModel.FilePath = filePath;
            }
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
        #endregion
    }
}
