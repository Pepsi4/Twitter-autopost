using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TweetSharp;

namespace MinimalMVVM
{
    public class TwitterUserModel : ObservableObject
    {
        public Uri PinUrl { get; set; }

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
                System.Windows.MessageBox.Show("yey");
                return true;
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            return false;
        }
    }
}
