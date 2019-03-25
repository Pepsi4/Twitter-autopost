using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using MinimalMVVM.ViewModels;
using System.Diagnostics;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinimalMVVM
{
    public struct TweetField
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }

    public class WindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #region constructors
        public WindowViewModel() { }
        public WindowViewModel(IWindowsController windowsController)
        {
            if (windowsController == null) throw new System.ArgumentNullException(nameof(windowsController));
            _windowsController = windowsController;
            _fileModel = new FileModel(windowsController);
            _timerModel = new TimerModel();
            _historyModel = new HistoryModel();
            _twitterUserModel = new TwitterUserModel();

            TwitterUserModel.IsLoggedInChanged += IsLoggedInHandler;

            CreateFile("Tweets.txt");
            OpenFile("Tweets.txt");

            StartTimer();
            //RefreshTimerModelHistory();
        }

        void IsLoggedInHandler(object sender, EventArgs e)
        {
            RaisePropertyChangedEvent(nameof(IsLoggedIn));
        }
        #endregion

        #region fields
        private TimerModel _timerModel;
        private IWindowsController _windowsController;
        private readonly FileModel _fileModel;
        private DateTime _dateTimeCurrent = DateTime.Today;
        private string _someText;
        private HistoryModel _historyModel;
        private TwitterUserModel _twitterUserModel;
        #endregion

        #region Properties
        public DateTime DateTimeCurrent
        {
            get
            {
                return _dateTimeCurrent;
            }
            set { _dateTimeCurrent = value; }
        }
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
        public int? SelectedLineIndex
        {
            get { return _historyModel.SelectedLineIndex; }

            set
            {
                _historyModel.SelectedLineIndex = value;
                RaisePropertyChangedEvent(nameof(SelectedLineIndex));
            }
        }
        public ObservableCollection<TweetField> History
        {
            get
            {
                return HistoryModel.History;
            }
            set
            {
                HistoryModel.History = value;
                RaisePropertyChangedEvent(nameof(History));
            }
        }
        public bool IsLoggedIn
        {
            get { return TwitterUserModel.IsLoggedIn; }
            set
            {
                TwitterUserModel.IsLoggedIn = value;
                Debug.WriteLine("IsLoggedIn " + value);
                RaisePropertyChangedEvent(nameof(IsLoggedIn));
            }
        }
        #endregion

        #region commands
        public ICommand ConvertTextCommand => new DelegateCommand(ConvertText, true);
        public ICommand DeleteLineFromTextCommand => new DelegateCommand(DeleteLineFromText, true);
        #endregion

        #region command-methods
        private void ConvertText()
        {
            if (IsDateBiggerThanToday(DateTimeCurrent) && IsInputNotEmpty())
            {
                _historyModel.AddToHistory(new TweetField()
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
            _historyModel.DeleteLineHistory();
            SaveChangesInFile();
        }

        #endregion

        #region helpers

        private void StartTimer()
        {
            _timerModel = new TimerModel();
            _timerModel.CreateTimer();
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

        private ObservableCollection<TweetField> BubbleSort(ObservableCollection<TweetField> mas)
        {
            TweetField temp;
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
            History = new ObservableCollection<TweetField>(_fileModel.GetTextFromFile());
        }

        private void SaveChangesInFile()
        {
            _fileModel.SaveChangesInFile(History);
        }

        private void ClearInput()
        {
            SomeText = string.Empty;
        }

        private void CreateFile(string path)
        {
            if (File.Exists(path))
            {
                Debug.WriteLine("Файл уже существует.");
            }
            else
            {
                FileStream file = File.Create(path);
                _windowsController.ShowMessage("Файл создан.");
            }
        }

        private void OpenFile(string filePath)
        {
            _historyModel.ClearHistory();

            if (filePath != null && filePath != "")
            {
                FileModel.TweetsPath = filePath;
                GetTextFromFile(filePath);
            }
            else
            {
                _windowsController.ShowMessage("Ошибка файла");
            }
        }
        #endregion
    }
}
