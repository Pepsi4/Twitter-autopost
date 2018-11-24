namespace MinimalMVVM.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            WindowViewModel windowViewModel = new WindowViewModel();
            this.DataContext = windowViewModel;
            InitializeComponent();
        }
    }
}
