namespace MinimalMVVM.Models
{
    public class TwitterUserModel : ObservableObject
    {
        public static string UserName { get; set; }

        public static long UserId { get; set; }

        public static string ConsumerKey { get; set; }

        public static string ConsumerSecret { get; set; }

        public static string AccessToken { get; set; }

        public static string AccessTokenSecret { get; set; }
    }
}
