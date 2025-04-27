using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace IHECBookzone.Desktop.Views
{
    public partial class BorrowingsPage : Page
    {
        private readonly BorrowingService _borrowingService;
        private readonly AuthService _authService;
        private readonly BookService _bookService;
        private ObservableCollection<Borrowing> _borrowings;
        private bool _isLoading = false;

        // Default constructor for designer
        public BorrowingsPage()
        {
            InitializeComponent();
            
            _borrowings = new ObservableCollection<Borrowing>();
            BorrowingsDataGrid.ItemsSource = _borrowings;
        }
        
        // Constructor with services injection
        public BorrowingsPage(AuthService authService, BookService bookService = null)
        {
            InitializeComponent();
            
            // Store services
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _bookService = bookService;
            
            // Initialize collections
            _borrowings = new ObservableCollection<Borrowing>();
            BorrowingsDataGrid.ItemsSource = _borrowings;
            
            // Create BorrowingService
            _borrowingService = new BorrowingService(_bookService, _authService);
            
            // Subscribe to events
            Loaded += BorrowingsPage_Loaded;
        }

        private async void BorrowingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBorrowingsAsync();
        }

        private async Task LoadBorrowingsAsync()
        {
            if (_isLoading) return;
            
            try
            {
                _isLoading = true;
                SetLoadingState(true);
                
                bool onlyActive = false;
                
                // Determine filter based on selected item
                if (FilterComboBox.SelectedItem != null)
                {
                    string filterText = ((ComboBoxItem)FilterComboBox.SelectedItem).Content.ToString();
                    onlyActive = filterText == "Active" || filterText == "Overdue";
                }
                
                var borrowings = await _borrowingService.GetUserBorrowingsAsync(onlyActive);
                
                // Apply additional filtering in-memory if needed
                if (FilterComboBox.SelectedItem != null)
                {
                    string filterText = ((ComboBoxItem)FilterComboBox.SelectedItem).Content.ToString();
                    
                    if (filterText == "Overdue")
                    {
                        borrowings = borrowings.Where(b => b.ReturnDate == null && b.DueDate < DateTime.Now).ToList();
                    }
                    else if (filterText == "Returned")
                    {
                        borrowings = borrowings.Where(b => b.ReturnDate != null).ToList();
                    }
                }
                
                // Note: DueStatus is a calculated read-only property, no need to set it
                
                _borrowings.Clear();
                foreach (var borrowing in borrowings)
                {
                    _borrowings.Add(borrowing);
                }
                
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load borrowings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                SetLoadingState(false);
            }
        }
        
        private void UpdateUI()
        {
            // Enable/disable controls based on data
            ReturnButton.IsEnabled = BorrowingsDataGrid.SelectedItem != null && 
                                    ((Borrowing)BorrowingsDataGrid.SelectedItem).ReturnDate == null;
        }
        
        private void SetLoadingState(bool isLoading)
        {
            StatusTextBlock.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            BorrowingsDataGrid.IsEnabled = !isLoading;
            FilterComboBox.IsEnabled = !isLoading;
            ReturnButton.IsEnabled = !isLoading && BorrowingsDataGrid.SelectedItem != null && 
                                    ((Borrowing)BorrowingsDataGrid.SelectedItem)?.ReturnDate == null;
            RefreshButton.IsEnabled = !isLoading;
        }
        
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadBorrowingsAsync();
        }
        
        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBorrowing = BorrowingsDataGrid.SelectedItem as Borrowing;
            
            if (selectedBorrowing != null && selectedBorrowing.ReturnDate == null)
            {
                var result = MessageBox.Show($"Are you sure you want to return '{selectedBorrowing.Book.Title}'?", 
                    "Confirm Return", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        SetLoadingState(true);
                        bool success = await _borrowingService.ReturnBookAsync(selectedBorrowing.Id);
                        
                        if (success)
                        {
                            await LoadBorrowingsAsync();
                            MessageBox.Show("Book has been returned successfully.", "Book Returned", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to return the book. Please try again.", "Error", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while returning the book: {ex.Message}", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        SetLoadingState(false);
                    }
                }
            }
        }
        
        private void BorrowingsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBorrowing = BorrowingsDataGrid.SelectedItem as Borrowing;
            ReturnButton.IsEnabled = selectedBorrowing != null && selectedBorrowing.ReturnDate == null;
        }
        
        private async void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadBorrowingsAsync();
        }
    }
} 