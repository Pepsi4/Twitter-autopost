namespace MinimalMVVM.Views
{

    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowViewModel windowViewModel = new WindowViewModel();
            this.DataContext = windowViewModel;

            TwitterBrowser twitterBrowser = new TwitterBrowser();
            twitterBrowser.Show();
        }
    }
}
