using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TweetSharp;

namespace MinimalMVVM
{
    public class TwitterUserModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));

            // With C# 6 this can be replaced with
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Uri PinUrl { get; set; }


        public static event EventHandler IsLoggedInChanged;

        private static bool isLoggedIn;
        public static bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                isLoggedIn = value;
                if (IsLoggedInChanged != null)
                {
                    IsLoggedInChanged(null, EventArgs.Empty);
                }
            }
        }

        public OAuthRequestToken Token
        {
            get; set;
        }

        public static TwitterService Service { get; private set; }

        public void CreateTwitterService()
        {
            TwitterClientInfo client = new TwitterClientInfo();
            client.ConsumerKey = "BBfi7kpS5sR2Ad5FrkrhuVe7y";
            client.ConsumerSecret = "BX5pLTGiQ4sqQXIHdEaQKL44wxmyes9E8KtyE7j08hmxhW1p0t";
            Service = new TwitterService(client);
            Token = Service.GetRequestToken();
            PinUrl = Service.GetAuthorizationUri(Token);
        }

        private string verifier;
        public string Verifier
        {
            get
            {
                return verifier;
            }
            set
            {
                verifier = value;
            }
        }

        public void PostTweet(string tweetText)
        {
            var result = Service.BeginSendTweet(new SendTweetOptions
            {
                Status = tweetText
            });

            Service.EndSendTweet(result);

            TwitterClientInfo twitterClientInfo = new TwitterClientInfo();
            TwitterService service = new TwitterService();
        }

        public bool TryLogin()
        {
            try
            {
                OAuthAccessToken accessToken =
           Service.GetAccessToken(Token, Verifier);

                Service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
                System.Windows.MessageBox.Show("Вы успешно вошли.");
                IsLoggedIn = true;
                return true;
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            return false;
        }
    }
}
