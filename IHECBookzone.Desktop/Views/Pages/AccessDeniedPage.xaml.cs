using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Services;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for AccessDeniedPage.xaml
    /// </summary>
    public partial class AccessDeniedPage : Page
    {
        private readonly AuthService _authService;

        public AccessDeniedPage()
        {
            InitializeComponent();
        }

        public AccessDeniedPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the home page
            if (_authService != null)
            {
                var bookService = new BookService();
                NavigationService?.Navigate(new HomePage(_authService, bookService));
            }
            else
            {
                // If no auth service is available, try to navigate to login
                NavigationService?.Navigate(new LoginPage(new AuthService(), new NavigationService()));
            }
        }
    }
} 