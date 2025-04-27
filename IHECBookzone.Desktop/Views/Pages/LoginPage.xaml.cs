using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using IHECBookzone.Desktop.Services;
using System.Windows.Input;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        public LoginPage(IAuthService authService, INavigationService navigationService)
        {
            if (authService == null) throw new ArgumentNullException(nameof(authService));
            if (navigationService == null) throw new ArgumentNullException(nameof(navigationService));

            InitializeComponent();
            _authService = authService;
            _navigationService = navigationService;
            
            // Set initial focus to email field
            Loaded += (s, e) => EmailTextBox.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearErrors();
                
                string email = EmailTextBox.Text.Trim();
                string password = PasswordBox.Password;
                
                if (string.IsNullOrEmpty(email))
                {
                    ShowError("Please enter your email address.");
                    return;
                }
                
                if (string.IsNullOrEmpty(password))
                {
                    ShowError("Please enter your password.");
                    return;
                }
                
                // Show loading state
                LoadingOverlay.Visibility = Visibility.Visible;
                LoginButton.IsEnabled = false;
                
                // Attempt login
                bool result = await _authService.SignInAsync(email, password);
                
                if (result)
                {
                    // Navigate to the main page on successful login
                    _navigationService.NavigateTo("Home");
                }
                else
                {
                    ShowError("Invalid email or password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Hide loading state
                LoadingOverlay.Visibility = Visibility.Collapsed;
                LoginButton.IsEnabled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo("RoleSelection");
        }

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo("Register");
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            // TODO: Implement forgot password functionality
            MessageBox.Show("Password reset functionality will be implemented in a future update.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GoogleSignInButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Google sign-in
            MessageBox.Show("Google sign-in will be implemented in a future update.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private void ClearErrors()
        {
            ErrorTextBlock.Text = string.Empty;
            ErrorTextBlock.Visibility = Visibility.Collapsed;
        }
    }
} 