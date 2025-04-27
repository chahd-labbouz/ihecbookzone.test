using System;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for BookDetailsPage.xaml
    /// </summary>
    public partial class BookDetailsPage : Page
    {
        private readonly Book _book;
        private readonly BookService _bookService;
        private readonly AuthService _authService;
        private readonly BorrowingService _borrowingService;

        public BookDetailsPage()
        {
            InitializeComponent();
        }

        public BookDetailsPage(Book book, BookService bookService, AuthService authService)
        {
            InitializeComponent();
            
            _book = book ?? throw new ArgumentNullException(nameof(book));
            _bookService = bookService;
            _authService = authService;
            
            // Create borrowing service with book service and auth service
            _borrowingService = new BorrowingService(_bookService, _authService);
            
            // Set DataContext to the book for binding
            DataContext = _book;
            
            // Initialize UI based on authentication status
            UpdateActionButtonVisibility();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to previous page
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private async void BorrowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_authService == null || !_authService.IsAuthenticated)
                {
                    MessageBox.Show("You need to be logged in to borrow books.", "Authentication Required", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SetLoadingState(true);
                
                bool success = await _borrowingService.BorrowBookAsync(_book.Id, _authService.CurrentUser.Id);
                
                if (success)
                {
                    MessageBox.Show($"You have successfully borrowed '{_book.Title}'.", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Refresh the book data or navigate back
                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Unable to borrow the book. Please try again later.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_authService == null || !_authService.IsAuthenticated)
                {
                    MessageBox.Show("You need to be logged in to reserve books.", "Authentication Required", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SetLoadingState(true);
                
                bool success = await _borrowingService.ReserveBookAsync(_book.Id, _authService.CurrentUser.Id);
                
                if (success)
                {
                    MessageBox.Show($"You have successfully reserved '{_book.Title}'.", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Refresh the book data or navigate back
                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Unable to reserve the book. Please try again later.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void UpdateActionButtonVisibility()
        {
            bool isAuthenticated = _authService != null && _authService.IsAuthenticated;
            
            // Update button visibility based on authentication and availability
            if (BorrowButton != null)
            {
                BorrowButton.Visibility = isAuthenticated && _book.IsAvailable ? 
                    Visibility.Visible : Visibility.Collapsed;
            }
            
            if (ReserveButton != null)
            {
                ReserveButton.Visibility = isAuthenticated && !_book.IsAvailable ? 
                    Visibility.Visible : Visibility.Collapsed;
            }
            
            if (SignInPrompt != null)
            {
                SignInPrompt.Visibility = !isAuthenticated ? 
                    Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SetLoadingState(bool isLoading)
        {
            if (LoadingOverlay != null)
            {
                LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            }
            
            if (BorrowButton != null)
            {
                BorrowButton.IsEnabled = !isLoading;
            }
            
            if (ReserveButton != null)
            {
                ReserveButton.IsEnabled = !isLoading;
            }
        }
    }
} 