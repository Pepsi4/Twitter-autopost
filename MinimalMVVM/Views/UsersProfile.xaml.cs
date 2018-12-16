using System.Windows;

namespace MinimalMVVM.Views
{
    public partial class UsersProfile : Window
    {
        public UsersProfile()
        {
            UsersProfileViewModel usersProfileViewModel = new UsersProfileViewModel();
            this.DataContext = usersProfileViewModel;
            InitializeComponent();
        }
    }
}
