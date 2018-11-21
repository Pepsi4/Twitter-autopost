namespace MinimalMVVM.Models
{
    public class TwitterUserModel : ObservableObject
    {
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private long _userId;
        public long UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
    }
}
