namespace MinimalMVVM.Views
{

    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowViewModel windowViewModel = new WindowViewModel();
            this.DataContext = windowViewModel;

            //TwitterBrowserView twitterBrowser = new TwitterBrowserView();
            //twitterBrowser.Show();
        }
    }
}
