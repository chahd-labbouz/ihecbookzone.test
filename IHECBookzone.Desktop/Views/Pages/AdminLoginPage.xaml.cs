using System;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Services;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for AdminLoginPage.xaml
    /// </summary>
    public partial class AdminLoginPage : Page
    {
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;

        public AdminLoginPage()
        {
            InitializeComponent();
        }

        public AdminLoginPage(AuthService authService, NavigationService navigationService)
        {
            InitializeComponent();
            _authService = authService ?? new AuthService();
            _navigationService = navigationService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ClearErrors();

            if (!ValidateForm())
                return;

            try
            {
                // Disable button and show loading state
                LoginButton.IsEnabled = false;
                LoginButton.Content = "Logging in...";
                MessageText.Text = string.Empty;

                // Attempt to login
                bool success = await _authService.SignInAsync(EmailTextBox.Text.Trim(), PasswordBox.Password);

                if (success && _authService.CurrentUser != null)
                {
                    // Check if user is an admin
                    if (_authService.CurrentUser.IsAdmin)
                    {
                        // Navigate to admin dashboard
                        var bookService = new BookService();
                        NavigationService?.Navigate(new AdminDashboardPage(_authService, bookService));
                    }
                    else
                    {
                        // Show error - user is not an admin
                        MessageText.Text = "Access denied. Your account does not have administrator privileges.";
                        LoginButton.IsEnabled = true;
                        LoginButton.Content = "Login";
                    }
                }
                else
                {
                    // Login failed
                    MessageText.Text = "Invalid email or password. Please try again.";
                    LoginButton.IsEnabled = true;
                    LoginButton.Content = "Login";
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = $"An error occurred: {ex.Message}";
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Login";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the role selection page
            if (_navigationService != null)
            {
                _navigationService.NavigateTo("RoleSelection");
            }
            else
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new LoginPage(new AuthService(), new NavigationService()));
                }
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Validate Email
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowError(EmailErrorText, "Email is required");
                isValid = false;
            }

            // Validate Password
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError(PasswordErrorText, "Password is required");
                isValid = false;
            }

            return isValid;
        }

        private void ShowError(TextBlock errorTextBlock, string message)
        {
            errorTextBlock.Text = message;
            errorTextBlock.Visibility = Visibility.Visible;
        }

        private void ClearErrors()
        {
            EmailErrorText.Visibility = Visibility.Collapsed;
            PasswordErrorText.Visibility = Visibility.Collapsed;
            MessageText.Text = string.Empty;
        }
    }
} 