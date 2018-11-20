using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MinimalMVVM.Models;
using System.IO;
using System.Linq;

namespace MinimalMVVM.ViewModels
{
    public class WindowViewModel : ObservableObject
    {
        #region fields
        private readonly FileModel file = new FileModel();
        private ObservableCollection<string> _history = new ObservableCollection<string>();
        private readonly FileModel fileModel = new FileModel();

        private string _someText;
        public string SomeText
        {
            get { return _someText; }
            set
            {
                _someText = value;
                RaisePropertyChangedEvent("SomeText");
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                RaisePropertyChangedEvent("FilePath");
            }
        }

        private int? _selectedLineIndex;
        public int? SelectedLineIndex
        {
            get { return _selectedLineIndex; }

            set
            {
                _selectedLineIndex = value;
                RaisePropertyChangedEvent("SelectedLineIndex");
            }
        }

        public IEnumerable<string> History
        {
            get { return _history; }
        }
        #endregion

        #region commands

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

        #region command-method
        private void ConvertText()
        {
            if (string.IsNullOrWhiteSpace(SomeText)) return;
            AddToHistory(_someText.ToUpper());
            SomeText = string.Empty;
            SaveChangesInFile();
        }

        private void DeleteLineFromText()
        {
            try
            {
                List<string> linesList = File.ReadAllLines(file.FilePath).ToList();
                linesList.RemoveAt((int)_selectedLineIndex);
                File.WriteAllLines(file.FilePath, _history);
            }
            catch (System.ArgumentNullException) {  }
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
        private void GetTextFromFile()
        {
            if (file.FilePath != null)
            {
                var fileStream = new FileStream(file.FilePath, FileMode.Open);
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
            if (file.FilePath != null)
            {
                File.WriteAllLines(file.FilePath, _history.ToList<string>());
            }
        }

        private void SelectFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                //FilePath = dlg.FileName;
                file.FilePath = dlg.FileName;
            }
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
