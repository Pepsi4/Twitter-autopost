using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Linq;
using TweetSharp;
using MinimalMVVM.ViewModels;
using System.Diagnostics;
using System;
using System.Windows.Threading;

namespace MinimalMVVM
{
    public struct TweetField
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
            fileModel = new FileModel(windowsController);
            timerModel = new TimerModel();
            historyModel = new HistoryModel();

            CreateFile("Tweets.txt");
            OpenFile("Tweets.txt");

            StartTimer();
            //RefreshTimerModelHistory();
        }
        #endregion

        #region Properties
        IWindowsController _windowsController;

        private readonly FileModel fileModel;
        private readonly TwitterUserModel twitterUserModel = new TwitterUserModel();

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


        public int? SelectedLineIndex
        {
            get { return historyModel.SelectedLineIndex; }

            set
            {
                historyModel.SelectedLineIndex = value;
                RaisePropertyChangedEvent(nameof(SelectedLineIndex));
            }
        }

        private HistoryModel historyModel;
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
        #endregion

        #region commands
        public ICommand ConvertTextCommand
        {
            get { return new DelegateCommand(ConvertText, true); }
        }

        //private bool CanExecuteAttachmentChecked() => true;

        public ICommand DeleteLineFromTextCommand => new DelegateCommand(DeleteLineFromText, true);
        #endregion

        #region command-methods

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
            historyModel.ClearHistory();

            if (filePath != null && filePath != "")
            {
                TweetsPath = filePath;
                GetTextFromFile(filePath);
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
                historyModel.AddToHistory(new TweetField()
                {
                    Text = SomeText,
                    Date = _dateTimeCurrent
                });

                //RefreshTimerModelHistory();

                ClearInput();
                BubbleSort(History);
                SaveChangesInFile();
            }
        }

        private void DeleteLineFromText()
        {
            historyModel.DeleteLineHistory();
            SaveChangesInFile();
        }

        #endregion

        #region helpers

        TimerModel timerModel;

        //private void RefreshTimerModelHistory()
        //{
        //timerModel.History = History;
        //}

        public void StartTimer()
        {
            timerModel = new TimerModel();
            timerModel.CreateTimer();
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
            History = new ObservableCollection<TweetField>(fileModel.GetTextFromFile());
        }

        public void SaveChangesInFile()
        {
            fileModel.SaveChangesInFile(History);
        }

        private void ClearInput()
        {
            SomeText = string.Empty;
        }
        #endregion
    }
}
