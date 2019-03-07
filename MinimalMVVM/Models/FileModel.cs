using System.Collections.Generic;
using System.IO;

namespace MinimalMVVM
{
    public class FileModel
    {
        public static string TweetsPath { get; set; } = "C://";

        public void SaveChangesInFile(string path, List<string> list)
        {
            try { File.WriteAllLines(path, list); }
            catch (System.UnauthorizedAccessException) { System.Windows.MessageBox.Show("Ошибка файла. Попробуйте создать или указать файл."); }
        }
    }
}
