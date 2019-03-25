using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalMVVM
{
    class HistoryModel
    {
        private static ObservableCollection<TweetField> _history = new ObservableCollection<TweetField>();

        public static ObservableCollection<TweetField> History
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        private int? _selectedLineIndex;

        public int? SelectedLineIndex
        {
            get { return _selectedLineIndex; }

            set
            {
                _selectedLineIndex = value;
            }
        }

        public void DeleteLineHistory()
        {
            if (_selectedLineIndex != null && _selectedLineIndex >= 0)
            {
                History.RemoveAt((int)_selectedLineIndex);
            }
        }

        public void DeleteTweet(TweetField tweet)
        {
            History.Remove(tweet);
        }

        public void AddToHistory(TweetField item)
        {
            if (!History.Contains(item))
            {
                History.Add(item);
            }
        }

        public void ClearHistory()
        {
            History.Clear();
        }
    }
}
