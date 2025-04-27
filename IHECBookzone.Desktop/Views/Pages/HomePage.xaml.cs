using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.Views.Pages;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly AuthService _authService;
        private readonly BookService _bookService;
        private readonly INavigationService _navigationService;
        private readonly BorrowingService _borrowingService;
        private User _currentUser;

        // Default constructor for design-time preview
        public HomePage()
        {
            InitializeComponent();
        }

        // Constructor for dependency injection
        public HomePage(AuthService authService = null, BookService bookService = null, INavigationService navigationService = null, BorrowingService borrowingService = null)
        {
            InitializeComponent();
            
            _authService = authService;
            _bookService = bookService;
            _navigationService = navigationService;
            _borrowingService = borrowingService;
            
            if (_authService?.IsAuthenticated == true)
            {
                _currentUser = _authService.CurrentUser;
            }
            
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Only proceed if auth service is available
                if (_authService == null)
                    return;
                    
                // Get current user from auth service
                _currentUser = _authService.CurrentUser;
                
                if (_currentUser != null)
                {
                    // Update welcome message with user's name
                    // Get the first name from the full name (take the first word)
                    string firstName = _currentUser.FullName?.Split(' ')[0] ?? "User";
                    WelcomeText.Text = $"Welcome back, {firstName}";
                    
                    // Load real data from the API
                    await UpdateDashboardStatsAsync();
                }
                else
                {
                    // If no user is logged in, navigate back to login
                    NavigateToLogin();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateDashboardStatsAsync()
        {
            try
            {
                if (_borrowingService == null || _currentUser == null)
                    return;

                // Get all borrowings for the current user
                var borrowings = await _borrowingService.GetUserBorrowingsAsync();
                
                if (borrowings != null)
                {
                    // Currently borrowed (active) books
                    var activeBorrowings = borrowings.Where(b => !b.IsReturned).ToList();
                    BorrowedCountText.Text = activeBorrowings.Count().ToString();
                
                    // Books due soon (next 5 days)
                    var dueSoonBooks = activeBorrowings
                        .Where(b => b.DaysRemaining > 0 && b.DaysRemaining <= 5)
                        .ToList();
                    DueSoonCountText.Text = dueSoonBooks.Count().ToString();
                    
                    // If we have any due soon books, update the text to show days remaining
                    if (dueSoonBooks.Count() > 0)
                    {
                        // Get the earliest due date
                        var earliestDueDays = dueSoonBooks.Min(b => b.DaysRemaining);
                        var daysText = earliestDueDays > 1 ? "days" : "day";
                        DueSoonCountText.Text = dueSoonBooks.Count().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating dashboard stats: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the library page
            if (_bookService != null && _authService != null)
            {
                if (_navigationService != null)
                {
                    _navigationService.NavigateTo<LibraryPage>();
                }
                else
                {
                    NavigationService?.Navigate(new LibraryPage(_bookService, _authService, new NavigationService()));
                }
            }
            else
            {
                MessageBox.Show("Book service or authentication service is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RecommendationsButton_Click(object sender, RoutedEventArgs e)
        {
            // In a full implementation, this would navigate to a recommendations page
            // For now, we'll just navigate to the library page
            if (_bookService != null && _authService != null)
            {
                if (_navigationService != null)
                {
                    _navigationService.NavigateTo<LibraryPage>();
                }
                else
                {
                    NavigationService?.Navigate(new LibraryPage(_bookService, _authService, new NavigationService()));
                }
            }
            else
            {
                MessageBox.Show("Book service or authentication service is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToLogin()
        {
            // Navigate back to login page
            if (_navigationService != null)
            {
                _navigationService.NavigateTo<LoginPage>();
            }
            else
            {
                NavigationService?.Navigate(new LoginPage(_authService, new NavigationService()));
            }
        }

        private void BorrowingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the borrowings page
            if (_authService != null)
            {
                if (_navigationService != null)
                {
                    _navigationService.NavigateTo<BorrowingsPage>();
                }
                else
                {
                    NavigationService?.Navigate(new BorrowingsPage(_authService, _bookService));
                }
            }
            else
            {
                MessageBox.Show("Authentication service is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewBorrowingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the borrowings page
            if (_authService != null)
            {
                if (_navigationService != null)
                {
                    _navigationService.NavigateTo<BorrowingsPage>();
                }
                else
                {
                    NavigationService?.Navigate(new BorrowingsPage(_authService, _bookService));
                }
            }
            else
            {
                MessageBox.Show("Authentication service is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BorrowedCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Navigate to the borrowings page with "active" filter
            if (_authService != null)
            {
                if (_navigationService != null)
                {
                    _navigationService.NavigateTo<BorrowingsPage>();
                }
                else
                {
                    NavigationService?.Navigate(new BorrowingsPage(_authService, _bookService));
                }
            }
        }
        
        private void DueSoonCard_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Navigate to the borrowings page with "due soon" filter
            if (_authService != null)
            {
                if (_navigationService != null)
                {
                    _navigationService.NavigateTo<BorrowingsPage>();
                }
                else
                {
                    NavigationService?.Navigate(new BorrowingsPage(_authService, _bookService));
                }
            }
        }
    }
} 