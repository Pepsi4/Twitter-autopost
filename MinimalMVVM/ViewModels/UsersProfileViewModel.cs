using System.Windows.Input;
using MinimalMVVM.ViewModels;
using System.Diagnostics;

namespace MinimalMVVM
{
    class UsersProfileViewModel : ObservableObject
    {
        public UsersProfileViewModel()
        {
            GetKeysFromFile();
        }

        private string ApiKeysFilePath = "ApiKyes.txt";

        public string ConsumerKey
        {
            get { return Models.TwitterUserModel.ConsumerKey; }
            set { Models.TwitterUserModel.ConsumerKey = value; }
        }

        public string ConsumerSecret
        {
            get { return Models.TwitterUserModel.ConsumerSecret; }
            set { Models.TwitterUserModel.ConsumerSecret = value; }
        }

        public string AccessToken
        {
            get { return Models.TwitterUserModel.AccessToken; }
            set { Models.TwitterUserModel.AccessToken = value; }
        }

        public string AccessTokenSecret
        {
            get { return Models.TwitterUserModel.AccessTokenSecret; }
            set { Models.TwitterUserModel.AccessTokenSecret = value; }
        }

        public ICommand SaveProfileCommand
        {
            get
            {
                return new DelegateCommand(SaveProfile, true);
            }
        }

        private void GetKeysFromFile()
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(ApiKeysFilePath);

                ConsumerKey = lines[0];
                ConsumerSecret = lines[1];
                AccessToken = lines[2];
                AccessTokenSecret = lines[3];
            }
            catch (System.IndexOutOfRangeException) { }
        }

        private void ClearFile()
        {
            System.IO.File.WriteAllText(ApiKeysFilePath, string.Empty);
        }

        private void SaveProfile()
        {
            ClearFile();

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(ApiKeysFilePath))
            {
                file.WriteLine(ConsumerKey);
                file.WriteLine(ConsumerSecret);
                file.WriteLine(AccessToken);
                file.WriteLine(AccessTokenSecret);
            }

            Debug.WriteLine(ConsumerKey);
            Debug.WriteLine(ConsumerSecret);
            Debug.WriteLine(AccessToken);
            Debug.WriteLine(AccessTokenSecret);
        }
    }
}
