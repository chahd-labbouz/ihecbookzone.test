using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IHECBookzone.Desktop.Views
{
    /// <summary>
    /// Interaction logic for DeleteBorrowingPage.xaml
    /// </summary>
    public partial class DeleteBorrowingPage : Page
    {
        private readonly IBorrowingService _borrowingService;
        private readonly Borrowing _borrowing;
        private readonly Action _refreshCallback;

        public DeleteBorrowingPage(Borrowing borrowing, IBorrowingService borrowingService, Action refreshCallback)
        {
            InitializeComponent();
            
            _borrowing = borrowing ?? throw new ArgumentNullException(nameof(borrowing));
            _borrowingService = borrowingService ?? throw new ArgumentNullException(nameof(borrowingService));
            _refreshCallback = refreshCallback ?? throw new ArgumentNullException(nameof(refreshCallback));
            
            DataContext = _borrowing;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteButton.IsEnabled = false;
                DeleteButton.Content = "Deleting...";
                
                await _borrowingService.DeleteBorrowingAsync(_borrowing.Id);
                
                MessageBox.Show("Borrowing record deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Execute the refresh callback to update the borrowings list
                _refreshCallback?.Invoke();
                
                // Navigate back
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete borrowing record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DeleteButton.IsEnabled = true;
                DeleteButton.Content = "Delete Record";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
} 