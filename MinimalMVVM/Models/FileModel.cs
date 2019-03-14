using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MinimalMVVM
{
    public class FileModel
    {
        IWindowsController _windowsController;

        public FileModel(IWindowsController windowsController)
        {
            _windowsController = windowsController;
        }

        public static string TweetsPath { get; set; } = "C://";

        public void SaveChangesInFile(ObservableCollection<TweetField> history)
        {
            try
            {
                using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(FileModel.TweetsPath, false))
                {
                    if (FileModel.TweetsPath != null)
                    {
                        foreach (TweetField item in history)
                        {
                            file.WriteLine(item.Text + " " + item.Date);
                        }
                    }

                    file.Close();
                }
            }
            catch (System.UnauthorizedAccessException) { System.Windows.MessageBox.Show("Ошибка файла. Попробуйте создать или указать файл."); }
            catch (System.Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        }

        public List<TweetField> GetTextFromFile()
        {
            if (FileModel.TweetsPath != null && FileModel.TweetsPath != "")
            {
                var fileStream = new FileStream(TweetsPath, FileMode.Open);
                var streamReader = new StreamReader(fileStream, System.Text.Encoding.Default);
                List<TweetField> tweetsList = new List<TweetField>();


                string data = null;
                while ((data = streamReader.ReadLine()) != null)
                {
                    Debug.WriteLine("Reading new string in file...");
                    tweetsList.Add(new TweetField()
                    {
                        Text = GetTextFromString(data),
                        Date = GetDateFromString(data)
                    });
                }

                fileStream.Close();
                streamReader.Close();

                return tweetsList;
            }
            else { _windowsController.ShowMessage("Ошибка файла"); }

            return null;
        }

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
    }
}
