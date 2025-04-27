using System;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.ViewModels;
using IHECBookzone.Desktop.Views;
using IHECBookzone.Desktop.Views.Pages;

namespace IHECBookzone.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AuthService _authService;
        private readonly BookService _bookService;
        private readonly INavigationService _navigationService;
        
        public string CurrentPageTag { get; set; } = "RoleSelection";
        public bool IsAuthenticated => _authService.IsAuthenticated;
        public bool IsAdmin => _authService.IsAuthenticated && _authService.CurrentUser.IsAdmin;
        public User CurrentUser => _authService.CurrentUser;

        public MainWindow(AuthService authService, BookService bookService, INavigationService navigationService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            InitializeComponent();
            DataContext = this;
            
            // Initialize NavigationService
            _navigationService.Initialize(MainFrame);
            
            // Subscribe to authentication events
            _authService.UserLoggedIn += OnUserLoggedIn;
            _authService.UserLoggedOut += OnUserLoggedOut;
            
            // Ensure user is logged out on startup
            if (_authService.IsAuthenticated)
            {
                _authService.SignOut();
            }
            
            // Force navigation to RoleSelection page first
            var result = _navigationService.NavigateTo("RoleSelection");
            if (!result)
            {
                System.Windows.MessageBox.Show("Failed to navigate to RoleSelectionPage. Please check your dependency injection and page registration.", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Prevent further navigation attempts if this fails
                return;
            }
        }

        private void NavigationButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                string tag = radioButton.Tag as string;
                if (!string.IsNullOrEmpty(tag))
                {
                    CurrentPageTag = tag;
                    NavigateToPage(tag);
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("Login");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _authService.SignOut();
        }

        private void OnUserLoggedIn(object sender, User user)
        {
            // Update UI bindings
            UpdateBindings();
            
            // Navigate to home page
            NavigateToPage("Home");
        }

        private void OnUserLoggedOut(object sender, EventArgs e)
        {
            // Update UI bindings
            UpdateBindings();
            
            // Navigate to role selection page
            NavigateToPage("RoleSelection");
        }

        private void UpdateBindings()
        {
            // Trigger property change notifications for binding updates
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(CurrentUser));
        }

        private void NavigateToPage(string pageName)
        {
            switch (pageName)
            {
                case "RoleSelection":
                    _navigationService.NavigateTo<RoleSelectionPage>();
                    break;
                    
                case "Login":
                    _navigationService.NavigateTo<LoginPage>();
                    break;
                    
                case "Register":
                    _navigationService.NavigateTo<RegisterPage>();
                    break;
                    
                case "Home":
                    _navigationService.NavigateTo<HomePage>();
                    break;
                    
                case "Library":
                    _navigationService.NavigateTo<LibraryPage>();
                    break;
                    
                case "Borrowings":
                    _navigationService.NavigateTo<BorrowingsPage>();
                    break;
                    
                case "Profile":
                    if (_authService.IsAuthenticated)
                    {
                        _navigationService.NavigateTo<ProfilePage>();
                    }
                    else
                    {
                        _navigationService.NavigateTo<LoginPage>();
                    }
                    break;
                    
                case "Admin":
                    if (_authService.IsAuthenticated && _authService.CurrentUser.IsAdmin)
                    {
                        _navigationService.NavigateTo<AdminDashboardPage>();
                    }
                    else
                    {
                        _navigationService.NavigateTo<AccessDeniedPage>();
                    }
                    break;
                    
                default:
                    _navigationService.NavigateTo<HomePage>();
                    break;
            }
        }

        // INotifyPropertyChanged implementation (simplified for brevity)
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
} 