using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using TweetSharp;

namespace MinimalMVVM
{
    class TimerModel
    {
        private DispatcherTimer timer = null;
        private int _duration = 15000;

        private TwitterUserModel twitterUserModel;

        public TimerModel()
        {
            History = new ObservableCollection<TweetField>();
            twitterUserModel = new TwitterUserModel();
        }

        public void CheckTweetsByDate()
        {
            Debug.WriteLine("CheckTweetsByDate...");
            foreach (var tweet in History)
            {
                Debug.WriteLine(tweet.Text);
                if (tweet.Date <= DateTime.Now)
                {
                    PostTweet(tweet.Text);
                }
            }
        }

        private void PostTweet(string tweetText)
        {
            twitterUserModel.PostTweet(tweetText);
        }

        public ObservableCollection<TweetField> History;

        public void CreateTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(_duration);
            timer.Tick += (sender, e) => CheckTweetsByDate();
            timer.Start();
        }
    }
}
