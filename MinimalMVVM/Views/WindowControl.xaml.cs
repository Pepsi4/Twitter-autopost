using Microsoft.Win32;

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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        public void ShowMessage(string msg)
        {
            System.Windows.MessageBox.Show(msg);
        }
    }
}
