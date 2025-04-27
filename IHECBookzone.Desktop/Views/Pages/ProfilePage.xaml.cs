using System.Windows.Controls;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.ViewModels;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private readonly ProfileViewModel _viewModel;

        public ProfilePage()
        {
            InitializeComponent();
        }

        public ProfilePage(AuthService authService)
        {
            InitializeComponent();
            
            _viewModel = new ProfileViewModel(authService);
            DataContext = _viewModel;
        }
    }
} 