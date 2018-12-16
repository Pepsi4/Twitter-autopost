using System.Collections.Generic;
using System.IO;

namespace MinimalMVVM
{
    public class FileModel
    {
        

        //string _filePath;
        //public string FilePath
        //{
        //    get { return _filePath; }
        //    set
        //    {
        //        _filePath = value;
        //    }
        //}

        //string  _allUsersFilePath = "C://";
        public static string TweetsPath { get; set; } = "C://";

        //string static _whiteUsersFilePath = "C://";
        //public static string WhiteUsersFilePath { get; set; } = "C://";

        public void SaveChangesInFile(string path, List<string> list)
        {
            try { File.WriteAllLines(path, list); }
            catch (System.UnauthorizedAccessException) { System.Windows.MessageBox.Show("Ошибка файла. Попробуйте создать или указать файл."); }
        }
    }
}
