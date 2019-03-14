using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TweetSharp;

namespace MinimalMVVM.Models
{
    public class TwitterUserModel : ObservableObject
    {
        public TwitterService Service { get; private set; }

        public static string UserName { get; set; }

        public static long UserId { get; set; }

        public static string ConsumerKey { get; set; }

        public static string ConsumerSecret { get; set; }

        public static string AccessToken { get; set; }

        public static string AccessTokenSecret { get; set; }

        private TwitterService CreateService()
        {
            if (TwitterUserModel.ConsumerKey == null ||
                TwitterUserModel.ConsumerSecret == null ||
                TwitterUserModel.AccessToken == null ||
                TwitterUserModel.AccessTokenSecret == null) { return null; }


            TwitterService service = new TwitterService(TwitterUserModel.ConsumerKey,
                TwitterUserModel.ConsumerSecret,
                TwitterUserModel.AccessToken,
                TwitterUserModel.AccessTokenSecret);

            return service;
        }

        public void CheckTweetsByDate(ObservableCollection<TextField> tweets)
        {
            Debug.WriteLine("CheckTweetsByDate...");
            foreach (var tweet in tweets)
            {
                if (tweet.Date <= DateTime.Now)
                {
                    PostTweet(tweet.Text);
                }
            }
        }

        private void PostTweet(string tweetText)
        {
            var result = Service.BeginSendTweet(new SendTweetOptions
            {
                Status = tweetText
            });

            Service.EndSendTweet(result);

            TwitterClientInfo twitterClientInfo = new TwitterClientInfo();
            //twitterClientInfo.
            TwitterService service = new TwitterService();
        }
    }
}
