using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for AdminDashboardPage.xaml
    /// </summary>
    public partial class AdminDashboardPage : Page
    {
        private readonly AuthService _authService;
        private readonly BookService _bookService;
        private readonly UserService _userService;
        private readonly BorrowingService _borrowingService;
        
        // Currently selected book for actions
        private Book _selectedBook;

        public AdminDashboardPage()
        {
            InitializeComponent();
        }

        public AdminDashboardPage(AuthService authService, BookService bookService)
        {
            InitializeComponent();
            
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            
            // Create other required services
            _userService = new UserService(_authService);
            _borrowingService = new BorrowingService(_bookService, _authService);
            
            Loaded += AdminDashboardPage_Loaded;
        }

        private async void AdminDashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verify the user is an admin
                if (_authService.CurrentUser == null || !_authService.CurrentUser.IsAdmin)
                {
                    MessageBox.Show("You must be an administrator to access this page.", "Access Denied", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService?.Navigate(new AccessDeniedPage(_authService));
                    return;
                }

                // Load the dashboard data
                await LoadDashboardStatisticsAsync();
                await LoadBooksAsync();
                await LoadBorrowingsAsync();
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadDashboardStatisticsAsync()
        {
            try
            {
                // Get book statistics
                var books = await _bookService.GetBooksAsync();
                TotalBooksText.Text = books.Count.ToString();
                
                // Get borrowing statistics
                var allBorrowings = await _borrowingService.GetAllBorrowingsAsync();
                var activeBorrowings = allBorrowings.Where(b => !b.IsReturned).ToList();
                var overdueBorrowings = activeBorrowings.Where(b => b.IsOverdue).ToList();
                
                BorrowedBooksText.Text = activeBorrowings.Count.ToString();
                OverdueBooksText.Text = overdueBorrowings.Count.ToString();
                
                // Get user statistics
                var users = await _userService.GetAllUsersAsync();
                TotalUsersText.Text = users.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadBooksAsync()
        {
            try
            {
                var books = await _bookService.GetBooksAsync();
                BooksDataGrid.ItemsSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading books: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadBorrowingsAsync()
        {
            try
            {
                // Get the selected filter
                string filter = "all";
                if (BorrowingFilterComboBox.SelectedIndex == 1)
                    filter = "active";
                else if (BorrowingFilterComboBox.SelectedIndex == 2)
                    filter = "overdue";
                
                // Get borrowings based on filter
                List<Borrowing> borrowings;
                
                if (filter == "active")
                    borrowings = await _borrowingService.GetActiveBorrowingsAsync();
                else if (filter == "overdue")
                    borrowings = await _borrowingService.GetOverdueBorrowingsAsync();
                else
                    borrowings = await _borrowingService.GetAllBorrowingsAsync();
                
                // Enhance borrowings with additional info for display
                var enhancedBorrowings = borrowings.Select(b => new
                {
                    Id = b.Id,
                    UserName = "User " + b.UserId.Substring(0, 5), // Simplified for demo
                    BookTitle = b.Book?.Title ?? "Unknown Book",
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    Status = b.Status,
                    IsReturned = b.IsReturned,
                    IsOverdue = b.IsOverdue
                }).ToList();
                
                BorrowingsDataGrid.ItemsSource = enhancedBorrowings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading borrowings: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadUsersAsync()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                UsersDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBook = BooksDataGrid.SelectedItem as Book;
        }

        private async void BorrowingFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reload borrowings with the new filter
            await LoadBorrowingsAsync();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for adding new book functionality
            MessageBox.Show("Add book functionality would be implemented here", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            // Get the book from the clicked row
            var button = sender as Button;
            var book = button.DataContext as Book;
            
            // Placeholder for edit book functionality
            MessageBox.Show($"Edit book functionality would be implemented here for book: {book?.Title}", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the book from the clicked row
                var button = sender as Button;
                var book = button.DataContext as Book;
                
                if (book == null) return;
                
                // Confirm deletion
                var result = MessageBox.Show($"Are you sure you want to delete the book '{book.Title}'?", 
                    "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    // Delete the book
                    var success = await _bookService.DeleteBookAsync(book.Id);
                    
                    if (success)
                    {
                        MessageBox.Show("Book deleted successfully.", "Success", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Refresh the books list
                        await LoadBooksAsync();
                        await LoadDashboardStatisticsAsync();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the book. It may be borrowed by users.", "Error", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting book: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void MarkReturned_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the borrowing from the clicked row
                var button = sender as Button;
                var borrowing = button.DataContext;
                var borrowingId = borrowing.GetType().GetProperty("Id").GetValue(borrowing).ToString();
                
                if (string.IsNullOrEmpty(borrowingId)) return;
                
                // Mark as returned
                var success = await _borrowingService.ReturnBookAsync(borrowingId);
                
                if (success)
                {
                    MessageBox.Show("Book marked as returned successfully.", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Refresh the borrowings list and statistics
                    await LoadBorrowingsAsync();
                    await LoadDashboardStatisticsAsync();
                }
                else
                {
                    MessageBox.Show("Failed to mark the book as returned.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error returning book: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            // Get the user from the clicked row
            var button = sender as Button;
            var user = button.DataContext as User;
            
            // Placeholder for edit user functionality
            MessageBox.Show($"Edit user functionality would be implemented here for user: {user?.FullName}", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void ToggleAdmin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the user from the clicked row
                var button = sender as Button;
                var user = button.DataContext as User;
                
                if (user == null) return;
                
                // Toggle admin status
                user.IsAdmin = !user.IsAdmin;
                
                // Update the user
                var success = await _userService.UpdateUserAsync(user);
                
                if (success)
                {
                    MessageBox.Show($"User '{user.FullName}' admin status updated successfully.", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Refresh the users list
                    await LoadUsersAsync();
                }
                else
                {
                    MessageBox.Show("Failed to update user admin status.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 