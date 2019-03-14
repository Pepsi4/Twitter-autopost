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
        #endregion

        #region commands
        

        public ICommand ConvertTextCommand
        {
            get { return new DelegateCommand(ConvertText, true); }
        }

        private bool CanExecuteAttachmentChecked() => true;

        private ICommand _createFileCommand;

        public ICommand CreateFileCommand
        {
            get
            {
                return _createFileCommand ?? (_createFileCommand = new DelegateCommand(param => CreateFile(param), CanExecuteAttachmentChecked()));
            }
        }

        public ICommand DeleteLineFromTextCommand => new DelegateCommand(DeleteLineFromText, true);

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
            if (IsDateBiggerThanToday(DateTimeCurrent) && IsInputNotEmpty())
            {
                AddToHistory(new TextField()
                {
                    Text = SomeText,
                    Date = _dateTimeCurrent
                });
                ClearInput();
                BubbleSort(History);
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
        private DateTime GetDateFromString(string str)
        {
            DateTime date = Convert.ToDateTime(str.Split(' ').Last());
            return date;
        }

        private string GetTextFromString(string str)
        {
            string[] strArray = str.Split(' ');

            int index = str.IndexOf(strArray[strArray.Length - 2]);
            return str.Substring(0, index);
        }

        public void StartTimer()
        {
            TwitterUserModel twitterUserModel = new TwitterUserModel();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(_duration);
            timer.Tick += (sender, e) => twitterUserModel.CheckTweetsByDate(History);
            timer.Start();
        }

        // For tests only. Do not use directly.
        public bool IsDateBiggerThanToday(DateTime dateTime)
        {
            if (dateTime > DateTime.Now)
            {
                return true;
            }
            else
            {
                try { _windowsController.ShowMessage("Дата выставлена неправильно"); }
                catch (NullReferenceException ex) { Debug.WriteLine(ex.Message); }
                return false;
            }
        }

        private ObservableCollection<TextField> BubbleSort(ObservableCollection<TextField> mas)
        {
            TextField temp;
            for (int i = 0; i < mas.Count; i++)
            {
                for (int j = i + 1; j < mas.Count; j++)
                {
                    if (mas[i].Date > mas[j].Date)
                    {
                        temp = mas[i];
                        mas[i] = mas[j];
                        mas[j] = temp;
                    }
                }
            }
            return mas;
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
                using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(FileModel.TweetsPath, false))
                {
                    if (FileModel.TweetsPath != null)
                    {
                        foreach (TextField item in _history)
                        {
                            file.WriteLine(item.Text + " " + item.Date);
                        }
                    }

                    file.Close();
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
            if (!_history.Contains(item))
            {
                _history.Add(item);
            }
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

namespace MinimalMVVM.Models
{
    public class TwitterTimerModel
    {
        private TwitterService service;
        public TwitterTimerModel(TwitterService service)
        {
            this.service = service;
        }

        public void CheckTweetsByDate(ObservableCollection<TextField> tweets)
        {
            foreach (var tweet in tweets)
            {
                if (tweet.Date <= DateTime.Now)
                {
                    PostTweet();
                }
            }
        }

        private void PostTweet()
        {
            service.BeginSendTweet(new SendTweetOptions
            {
                Status = "test"
            });
        }
    }
}
