using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.Commands;

namespace IHECBookzone.Desktop.ViewModels
{
    public class BorrowingHistoryViewModel : INotifyPropertyChanged
    {
        private readonly BorrowingService _borrowingService;
        private readonly UserService _userService;
        private readonly AuthService _authService;
        private ObservableCollection<Borrowing> _borrowings;
        private bool _isLoading;
        private bool _showOnlyActive;
        private string _errorMessage;
        private Borrowing _selectedBorrowing;

        public BorrowingHistoryViewModel(UserService userService, AuthService authService, BookService bookService = null)
        {
            _userService = userService;
            _authService = authService;
            
            // Create BorrowingService with BookService for fetching book details, and AuthService for token
            _borrowingService = new BorrowingService(bookService, _authService);
            
            Borrowings = new ObservableCollection<Borrowing>();
            ReturnBookCommand = new RelayCommand(ReturnBook, CanReturnBook);
            RefreshCommand = new RelayCommand(async _ => await LoadBorrowingsAsync());
            ToggleActiveFilterCommand = new RelayCommand(async _ => 
            {
                ShowOnlyActive = !ShowOnlyActive;
                await LoadBorrowingsAsync();
            });
            
            // Initialize the ViewModel with data
            InitializeAsync();
        }

        public ObservableCollection<Borrowing> Borrowings
        {
            get => _borrowings;
            set
            {
                _borrowings = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
        
        public Borrowing SelectedBorrowing
        {
            get => _selectedBorrowing;
            set
            {
                _selectedBorrowing = value;
                OnPropertyChanged();
                ((RelayCommand)ReturnBookCommand).RaiseCanExecuteChanged();
            }
        }

        public bool ShowOnlyActive
        {
            get => _showOnlyActive;
            set
            {
                _showOnlyActive = value;
                OnPropertyChanged();
            }
        }

        public ICommand ReturnBookCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleActiveFilterCommand { get; }

        public async Task LoadBorrowingsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                Borrowings.Clear();

                var currentUser = _userService.CurrentUser;
                if (currentUser == null)
                {
                    ErrorMessage = "You must be logged in to view your borrowing history.";
                    return;
                }

                var borrowings = await _borrowingService.GetUserBorrowingsAsync(!ShowOnlyActive);
                
                if (borrowings == null || !borrowings.Any())
                {
                    ErrorMessage = "No borrowing records found.";
                    return;
                }

                // Sort borrowings by date (newest first) and status (active first)
                var sortedBorrowings = borrowings
                    .OrderByDescending(b => b.IsReturned ? 0 : 1)
                    .ThenByDescending(b => b.BorrowDate);

                foreach (var borrowing in sortedBorrowings)
                {
                    Borrowings.Add(borrowing);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading borrowing history: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void ReturnBook(object parameter)
        {
            if (SelectedBorrowing == null) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var result = await _borrowingService.ReturnBookAsync(SelectedBorrowing.Id);
                if (result)
                {
                    // Refresh the list
                    await LoadBorrowingsAsync();
                }
                else
                {
                    ErrorMessage = "Failed to return the book. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error returning book: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanReturnBook(object parameter)
        {
            return SelectedBorrowing != null && !SelectedBorrowing.IsReturned;
        }

        private async void InitializeAsync()
        {
            try
            {
                await LoadBorrowingsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load initial data: {ex.Message}";
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
} 