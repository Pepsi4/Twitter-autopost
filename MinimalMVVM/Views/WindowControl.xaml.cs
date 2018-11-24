namespace MinimalMVVM.Views
{
    public partial class WindowControl : IWindowsController
    {
        public WindowControl()
        {
            WindowViewModel windowViewModel = new WindowViewModel(this);
            this.DataContext = windowViewModel;
            InitializeComponent();
        }

        public string ShowFileDialog()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            return dlg.FileName;
        }
    }
}
