using System.Windows;

namespace MinimalMVVM.Views
{
    /// <summary>
    /// Interaction logic for TwitterBrowser.xaml
    /// </summary>
    public partial class TwitterBrowserView : Window
    {
        public TwitterBrowserView()
        {
            InitializeComponent();
            TwitterBrowserViewModel twitterBrowserViewModel = new TwitterBrowserViewModel();
            this.DataContext = twitterBrowserViewModel;
        }
    }
}
