using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.Models;
using System.Text.RegularExpressions;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        
        private readonly Regex _emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        // Default parameterless constructor for navigation
        public RegisterPage()
        {
            InitializeComponent();
            _authService = new AuthService();
            _navigationService = new Services.NavigationService();
        }

        public RegisterPage(IAuthService authService, INavigationService navigationService)
        {
            InitializeComponent();
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // Set initial focus to email field
            Loaded += (s, e) => EmailTextBox.Focus();
        }
        
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearAllErrors();
                
                // Validate all fields
                bool isValid = true;
                
                // Email validation
                string email = EmailTextBox.Text.Trim();
                if (string.IsNullOrEmpty(email) || !_emailRegex.IsMatch(email))
                {
                    ShowError(EmailErrorText, "Please enter a valid email address.");
                    isValid = false;
                }
                
                // Password validation
                string password = PasswordBox.Password;
                if (string.IsNullOrEmpty(password) || password.Length < 6)
                {
                    ShowError(PasswordErrorText, "Password must be at least 6 characters.");
                    isValid = false;
                }
                
                // Confirm password validation
                string confirmPassword = ConfirmPasswordBox.Password;
                if (password != confirmPassword)
                {
                    ShowError(ConfirmPasswordErrorText, "Passwords don't match.");
                    isValid = false;
                }
                
                // Full name validation
                string fullName = FullNameTextBox.Text.Trim();
                if (string.IsNullOrEmpty(fullName) || fullName.Length < 3)
                {
                    ShowError(FullNameErrorText, "Please enter your full name (at least 3 characters).");
                    isValid = false;
                }
                
                // Level of study validation
                if (LevelOfStudyComboBox.SelectedItem == null)
                {
                    ShowError(LevelOfStudyErrorText, "Please select your level of study.");
                    isValid = false;
                }
                
                // Field of study validation
                if (FieldOfStudyComboBox.SelectedItem == null)
                {
                    ShowError(FieldOfStudyErrorText, "Please select your field of study.");
                    isValid = false;
                }
                
                if (!isValid) return;
                
                // Show loading state
                LoadingOverlay.Visibility = Visibility.Visible;
                RegisterButton.IsEnabled = false;
                
                // Create user object
                var user = new User
                {
                    Email = email,
                    FullName = fullName,
                    PhoneNumber = PhoneNumberTextBox.Text.Trim(),
                    LevelOfStudy = (LevelOfStudyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    FieldOfStudy = (FieldOfStudyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()
                };
                
                // Attempt registration
                bool result = await _authService.RegisterAsync(user, password);
                
                if (result)
                {
                    MessageBox.Show("Registration successful! Please check your email to confirm your account.", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Navigate to login page
                    _navigationService.NavigateTo("Login");
                }
                else
                {
                    ShowError(ErrorMessageText, "Registration failed. Please try again or contact support.");
                }
            }
            catch (Exception ex)
            {
                ShowError(ErrorMessageText, $"An error occurred: {ex.Message}");
            }
            finally
            {
                // Hide loading state
                LoadingOverlay.Visibility = Visibility.Collapsed;
                RegisterButton.IsEnabled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService?.NavigateTo("Login");
        }

        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            _navigationService?.NavigateTo("Login");
        }

        private void GoogleSignUpButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Google sign-up functionality
            MessageBox.Show("Google Sign-Up functionality will be implemented in a future update.", 
                "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowError(TextBlock errorTextBlock, string message)
        {
            errorTextBlock.Text = message;
            errorTextBlock.Visibility = Visibility.Visible;
        }

        private void ClearAllErrors()
        {
            EmailErrorText.Visibility = Visibility.Collapsed;
            PasswordErrorText.Visibility = Visibility.Collapsed;
            ConfirmPasswordErrorText.Visibility = Visibility.Collapsed;
            FullNameErrorText.Visibility = Visibility.Collapsed;
            PhoneNumberErrorText.Visibility = Visibility.Collapsed;
            LevelOfStudyErrorText.Visibility = Visibility.Collapsed;
            FieldOfStudyErrorText.Visibility = Visibility.Collapsed;
            ErrorMessageText.Visibility = Visibility.Collapsed;
        }
    }
} 