using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MinimalMVVM.Models;
using System.IO;
using System.Linq;
using TweetSharp;
using MinimalMVVM.ViewModels;
using System.Diagnostics;
using System;
using System.Windows.Threading;

namespace MinimalMVVM
{
    public struct TextField
    {

        public string Text { get; set; }
        public DateTime Date { get; set; }
    }

    public class WindowViewModel : ObservableObject
    {
        #region constructors
        public WindowViewModel() { }

        public WindowViewModel(IWindowsController windowsController)
        {
            if (windowsController == null) throw new System.ArgumentNullException(nameof(windowsController));
            _windowsController = windowsController;
            this.StartTimerCommand = new DelegateCommand(this.StartTimer, true);
            this.StopTimerCommand = new DelegateCommand(this.StopTimer, true);

            StartTimer();
        }
        #endregion

        #region Properties
        IWindowsController _windowsController;

        private readonly FileModel fileModel = new FileModel();
        private readonly TwitterUserModel twitterUserModel = new TwitterUserModel();

        private ObservableCollection<TextField> _history = new ObservableCollection<TextField>();

        private DispatcherTimer timer = null;

        private int _duration = 1000;

        public ICommand StartTimerCommand
        {
            get;
            set;
        }

        public ICommand StopTimerCommand
        {
            get;
            set;
        }

        private DateTime _dateTimeCurrent = DateTime.Today;
        public DateTime DateTimeCurrent
        {
            get
            {
                return _dateTimeCurrent;
            }
            set { _dateTimeCurrent = value; }
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

        public string TweetsPath
        {
            get
            {
                return FileModel.TweetsPath;
            }
            set
            {
                FileModel.TweetsPath = value;
                RaisePropertyChangedEvent(nameof(TweetsPath));
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

        public ObservableCollection<TextField> History
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
                RaisePropertyChangedEvent(nameof(History));
            }
        }

        private static string ConsumerKey = "";
        private static string ConsumerSecret = "";
        private static string _accessToken = "";
        private static string _accessTokenSecret = "";

        private static TwitterService service = new TwitterService(ConsumerKey, ConsumerSecret, _accessToken, _accessTokenSecret);

        #endregion

        #region commands

        public ICommand OpenUsersListFormCommand
        {
            get { return new DelegateCommand(OpenUsersListForm, true); }
        }

        public ICommand ConvertTextCommand
        {
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

        private void OpenUsersListForm()
        {
            Views.UsersProfile usersListForm = new Views.UsersProfile();
            usersListForm.Show();
        }

        private void CreateFile(object obj)
        {
            string path = obj.ToString();

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
                TweetsPath = filePath;
                GetTextFromFile(buttonName.ToString());
            }
            else
            {
                _windowsController.ShowMessage("Ошибка файла");
            }
        }

        private void ConvertText()
        {
            if (IsDateBiggerThanToday() && IsInputNotEmpty())
            {
                AddToHistory(new TextField()
                {
                    Text = SomeText,
                    Date = _dateTimeCurrent
                });
                ClearInput();
                SaveChangesInFile();
            }
        }

        private void DeleteLineFromText()
        {
            try
            {
                List<string> linesList = File.ReadAllLines(FileModel.TweetsPath).ToList();
                linesList.RemoveAt((int)_selectedLineIndex);
            }
            catch (System.ArgumentNullException) { }
            catch (System.ArgumentOutOfRangeException) { }

            DeleteLineHistory();
            SaveChangesInFile();
        }

        #endregion

        #region helpers
        public void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(_duration);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }

        public void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void TimerTick(object send, EventArgs e)
        {
            TimerAction();
        }

        private DateTime GetDateFromString(string str)
        {
            DateTime date = Convert.ToDateTime(str.Split(' ').Last());
            return date;
        }

        private string GetTextFromString(string str)
        {
            int index = str.IndexOf(str.Split(' ').Last());
            return str.Substring(0, index);
        }


        static List<DateTime> SortAscending(List<DateTime> list)
        {
            list.Sort((x, y) => DateTime.Compare(x, y));
            return list;
        }

        private string[] SortArrayByDate(DateTime[] dates, List<string> list)
        {
            var result = dates.OrderBy(d => d).ToArray();
            var listResult = new string[list.Count];

            for (int i = 0; i < result.Length; i++)
            {
                listResult[i] = list[Array.FindIndex(ConvertDateToString(result), row => row.Contains(list[i])) - 1];
            }

            return listResult;
        }

        private string[] ConvertDateToString(DateTime[] date)
        {
            string[] str = new string[date.Length];
            for (int i = 0; i < date.Length; i++)
            {
                str[i] = date[i].ToString();
            }

            return str;
        }

        //tweetsharp methods are here.
        private void TimerAction()
        {
            //if (DateTimeCurrent.Day < DateTime.Now.Day ||
            //     ((DateTimeCurrent.Day == DateTime.Now.Day) && (DateTimeCurrent.Hour < DateTime.Now.Hour))
            //     || ((DateTimeCurrent.Day == DateTime.Now.Day) && (DateTimeCurrent.Hour == DateTime.Now.Hour && DateTimeCurrent.Minute < DateTime.Now.Minute)))
            //{
            //    var service = CreateService();
            //    if (service != null)
            //    {

            //    }
            //    else
            //    {
            //        //_windowsController.ShowMessage("Неверно указаны данные пользователя. Попробуйте проверить правильность данных.");
            //    }
            //}
        }

        private TwitterService CreateService()
        {
            if (TwitterUserModel.ConsumerKey == null ||
                TwitterUserModel.ConsumerSecret == null ||
                TwitterUserModel.AccessToken == null ||
                TwitterUserModel.AccessTokenSecret == null) { return null; }


            TwitterService service = new TwitterService(TwitterUserModel.ConsumerKey,
                TwitterUserModel.ConsumerSecret,
                TwitterUserModel.AccessToken,
                TwitterUserModel.AccessTokenSecret);

            return service;
        }

        //ошибка на месяцах
        private bool IsDateBiggerThanToday()
        {
            if (DateTimeCurrent.Day > DateTime.Now.Day ||
                ((DateTimeCurrent.Day == DateTime.Now.Day) && (DateTimeCurrent.Hour > DateTime.Now.Hour))
                || ((DateTimeCurrent.Day == DateTime.Now.Day) && (DateTimeCurrent.Hour == DateTime.Now.Hour && DateTimeCurrent.Minute > DateTime.Now.Minute)))
            {
                return true;
            }
            else
            {
                _windowsController.ShowMessage("Дата выставлена неправильно");
                return false;
            }
        }

        private bool IsInputNotEmpty()
        {
            if (SomeText != "" && SomeText != null && SomeText != " ")
            { return true; }
            else
            {
                _windowsController.ShowMessage("Ошибка ввода строки");
                return false;
            }
        }

        private void GetTextFromFile(string listName)
        {
            if (FileModel.TweetsPath != null && FileModel.TweetsPath != "")
            {
                var fileStream = new FileStream(FileModel.TweetsPath, FileMode.Open);
                var streamReader = new StreamReader(fileStream, System.Text.Encoding.Default);

                string data = null;
                while ((data = streamReader.ReadLine()) != null)
                {
                    Debug.WriteLine("Reading new string in file...");
                    AddToHistory(new TextField()
                    {
                        Text = GetTextFromString(data),
                        Date = GetDateFromString(data)
                    });
                }

                fileStream.Close();
                streamReader.Close();
            }
            else { _windowsController.ShowMessage("Ошибка файла"); }
        }

        public void SaveChangesInFile()
        {
            try
            {
                if (FileModel.TweetsPath != null)
                {
                    foreach (TextField item in _history)
                    {
                        File.WriteAllText(FileModel.TweetsPath, item.Text + " " + item.Date);
                    }
                }
            }
            catch (System.UnauthorizedAccessException) { _windowsController.ShowMessage("Ошибка файла. Попробуйте создать или указать файл."); }
            catch (System.Exception ex) { _windowsController.ShowMessage(ex.Message); }
        }

        private void ClearInput()
        {
            SomeText = string.Empty;
        }

        private void ClearHistory()
        {
            _history.Clear();
        }

        private void AddToHistory(TextField item)
        {
            if (!_history.Contains(item)) //item.Text + " " + item.Date
            {
                _history.Add(item);
                Debug.WriteLine(_history.Count);

                History = _history;
            }
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
