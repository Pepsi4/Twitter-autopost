using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace MinimalMVVM
{
    class TimerModel
    {
        private DispatcherTimer timer = null;
        private int _duration = 15000;

        private TwitterUserModel twitterUserModel;
        FileModel fileModel;

        public TimerModel()
        {
            twitterUserModel = new TwitterUserModel();
            fileModel = new FileModel();
        }

        public void CheckTweetsByDate()
        {
            if (TwitterUserModel.IsLoggedIn)
            {
                Debug.WriteLine("CheckTweetsByDate...");

                for (int i = 0; i < HistoryModel.History.Count; i++)
                {
                    Debug.WriteLine(HistoryModel.History[i].Text);
                    if (HistoryModel.History[i].Date <= DateTime.Now)
                    {
                        PostTweet(HistoryModel.History[i].Text);
                        HistoryModel historyModel = new HistoryModel();
                        historyModel.DeleteTweet(HistoryModel.History[i]);
                        fileModel.SaveChangesInFile(HistoryModel.History);

                        break;
                    }
                }
            }
            else
            {
                Debug.WriteLine("You are not logged in...");
            }
        }

        private void PostTweet(string tweetText)
        {
            twitterUserModel.PostTweet(tweetText);
        }

        public void CreateTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(_duration);
            timer.Tick += (sender, e) => CheckTweetsByDate();
            timer.Start();
        }
    }
}
