using System.Windows;

namespace MinimalMVVM.Views
{
    /// <summary>
    /// Interaction logic for TwitterBrowser.xaml
    /// </summary>
    public partial class TwitterBrowser : Window
    {
        public TwitterBrowser()
        {
            InitializeComponent();
            TwitterBrowserViewModel twitterBrowserViewModel = new TwitterBrowserViewModel();
            this.DataContext = twitterBrowserViewModel;
        }
    }
}
