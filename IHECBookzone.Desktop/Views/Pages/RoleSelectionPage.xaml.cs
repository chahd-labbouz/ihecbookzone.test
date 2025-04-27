using System;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Services;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for RoleSelectionPage.xaml
    /// </summary>
    public partial class RoleSelectionPage : Page
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        public RoleSelectionPage(IAuthService authService, INavigationService navigationService)
        {
            try
            {
                if (authService == null) throw new ArgumentNullException(nameof(authService));
                if (navigationService == null) throw new ArgumentNullException(nameof(navigationService));

                InitializeComponent();
                _authService = authService;
                _navigationService = navigationService;

                System.Windows.MessageBox.Show("RoleSelectionPage loaded");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading RoleSelectionPage: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void RegisterStudentButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo("Register");
        }

        private void LoginStudentButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo("Login");
        }

        private void AdminLoginButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo("AdminLogin");
        }
    }
} 