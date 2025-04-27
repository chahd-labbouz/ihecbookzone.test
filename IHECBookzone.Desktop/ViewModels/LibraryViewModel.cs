using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IHECBookzone.Desktop.Commands;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.Utils;
using IHECBookzone.Desktop.Views.Pages;

// Fix for ambiguous NotificationService reference
using NotificationServiceUtils = IHECBookzone.Desktop.Utils.NotificationService;
using NotificationType = IHECBookzone.Desktop.Utils.NotificationType;

namespace IHECBookzone.Desktop.ViewModels
{
    public class LibraryViewModel : INotifyPropertyChanged
    {
        private readonly BookService _bookService;
        private readonly AuthService _authService;
        private readonly BorrowingService _borrowingService;
        private readonly INavigationService _navigationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        
        private ObservableCollection<Book> _books;
        private List<string> _categories;
        private string _searchTerm;
        private string _selectedCategory;
        private string _selectedAcademicLevel;
        private bool? _availabilityFilter;
        private Book _selectedBook;
        private bool _isLoading;
        private string _sortOption;
        private Dictionary<string, bool> _categoryFilters;
        private Dictionary<string, bool> _languageFilters;
        private ObservableCollection<Book> _filteredBooks;
        
        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public ObservableCollection<Book> FilteredBooks
        {
            get => _filteredBooks;
            set => SetProperty(ref _filteredBooks, value);
        }

        public List<string> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public List<string> AcademicLevels { get; } = new List<string> 
        { 
            "All Levels", 
            "Licence", 
            "Master", 
            "Doctorat" 
        };

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    SearchCommand.Execute(null);
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    SearchCommand.Execute(null);
                }
            }
        }

        public string SelectedAcademicLevel
        {
            get => _selectedAcademicLevel;
            set
            {
                if (SetProperty(ref _selectedAcademicLevel, value))
                {
                    SearchCommand.Execute(null);
                }
            }
        }

        public bool? AvailabilityFilter
        {
            get => _availabilityFilter;
            set
            {
                if (SetProperty(ref _availabilityFilter, value))
                {
                    SearchCommand.Execute(null);
                }
            }
        }

        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        
        public string SortOption
        {
            get => _sortOption;
            set
            {
                if (SetProperty(ref _sortOption, value))
                {
                    ApplySorting();
                }
            }
        }
        
        public Dictionary<string, bool> CategoryFilters
        {
            get => _categoryFilters;
            set => SetProperty(ref _categoryFilters, value);
        }
        
        public Dictionary<string, bool> LanguageFilters
        {
            get => _languageFilters;
            set => SetProperty(ref _languageFilters, value);
        }
        
        public bool HasNoBooks => FilteredBooks.Count == 0 && !IsLoading;

        public ICommand SearchCommand { get; }
        public ICommand BorrowCommand { get; }
        public ICommand ReserveCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand ApplyFiltersCommand { get; }

        public LibraryViewModel(BookService bookService, AuthService authService, INavigationService navigationService, INotificationService notificationService = null)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _logger = new LoggerWrapper(); // Use logger wrapper for proper logging
            _notificationService = notificationService ?? Services.NotificationService.Instance;
            
            // Create a BorrowingService with the current user ID, BookService, and AuthService
            _borrowingService = new BorrowingService(_bookService, _authService);
            
            Books = new ObservableCollection<Book>();
            FilteredBooks = new ObservableCollection<Book>();
            Categories = new List<string> { "All Categories" };
            SelectedCategory = "All Categories";
            SelectedAcademicLevel = "All Levels";
            SortOption = "Most Popular";
            
            // Initialize filter dictionaries
            CategoryFilters = new Dictionary<string, bool>
            {
                { "Finance", false },
                { "Management", false },
                { "Marketing", false },
                { "Economics", false },
                { "Accounting", false },
                { "BI", false },
                { "BigData", false }
            };
            
            LanguageFilters = new Dictionary<string, bool>
            {
                { "English", false },
                { "French", false },
                { "Arabic", false }
            };
            
            SearchCommand = new RelayCommand(async _ => await SearchBooksAsync());
            BorrowCommand = new RelayCommand(async book => await BorrowBookAsync(book as Book), CanBorrowBook);
            ReserveCommand = new RelayCommand(async book => await ReserveBookAsync(book as Book), CanReserveBook);
            ViewDetailsCommand = new RelayCommand(book => ViewBookDetails(book as Book));
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            ApplyFiltersCommand = new RelayCommand(async _ => await ApplyFiltersAsync());
            
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadCategoriesAsync();
            await SearchBooksAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            IsLoading = true;
            try
            {
                Categories = await _bookService.GetCategoriesAsync();
                
                // Sync the retrieved categories with our filter dictionary
                List<string> categories = Categories
                    .Where(c => c != "All Categories")
                    .ToList();
                
                // Update the CategoryFilters dictionary to match available categories
                CategoryFilters.Clear();
                foreach (var category in categories)
                {
                    CategoryFilters[category] = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Error loading categories");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchBooksAsync()
        {
            IsLoading = true;
            try
            {
                string academicLevel = SelectedAcademicLevel == "All Levels" ? null : SelectedAcademicLevel;
                
                var books = await _bookService.GetBooksAsync(
                    SearchTerm,
                    SelectedCategory,
                    academicLevel,
                    AvailabilityFilter);
                
                Books.Clear();
                foreach (var book in books)
                {
                    // Set background color based on category for UI visualization
                    SetBookBackgroundColor(book);
                    Books.Add(book);
                }
                
                // Apply sorting after loading books
                ApplySorting();
                
                // Apply current filters to populate the FilteredBooks collection
                await ApplyFiltersAsync();
                
                // Notify UI that HasNoBooks may have changed
                OnPropertyChanged(nameof(HasNoBooks));
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Error searching books");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private void SetBookBackgroundColor(Book book)
        {
            // Assign a background color based on book category
            if (book == null) return;
            
            // Get a deterministic color based on the category name
            string category = book.Category ?? "Uncategorized";
            
            // Generate a color based on the hash of the category name
            int hash = category.GetHashCode();
            byte r = (byte)((hash & 0xFF0000) >> 16);
            byte g = (byte)((hash & 0x00FF00) >> 8);
            byte b = (byte)(hash & 0x0000FF);
            
            // Make sure the color is light enough for text visibility
            r = (byte)Math.Min(255, r + 120);
            g = (byte)Math.Min(255, g + 120);
            b = (byte)Math.Min(255, b + 120);
            
            // Set the background color
            book.BackgroundColor = new SolidColorBrush(Color.FromRgb(r, g, b));
        }
        
        private async Task ApplyFiltersAsync()
        {
            IsLoading = true;
            try
            {
                // Apply category filters
                var selectedCategories = CategoryFilters
                    .Where(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .ToList();
                
                // Apply language filters
                var selectedLanguages = LanguageFilters
                    .Where(kvp => kvp.Value)
                    .Select(kvp => kvp.Key)
                    .ToList();
                
                // Start with all books from main collection
                FilteredBooks.Clear();
                var filteredBooks = Books.ToList();
                
                // Apply category filters if any are selected
                if (selectedCategories.Any())
                {
                    filteredBooks = filteredBooks.Where(book => selectedCategories.Contains(book.Category)).ToList();
                }
                
                // Apply language filters if any are selected
                if (selectedLanguages.Any())
                {
                    filteredBooks = filteredBooks.Where(book => selectedLanguages.Contains(book.Language)).ToList();
                }
                
                // Update the filtered collection
                foreach (var book in filteredBooks)
                {
                    FilteredBooks.Add(book);
                }
                
                // Apply sorting to the filtered books
                ApplySortingToFiltered();
                
                // Notify UI that HasNoBooks may have changed
                OnPropertyChanged(nameof(HasNoBooks));
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Error applying filters");
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private void ApplySorting()
        {
            if (Books.Count == 0) return;
            
            var sortedList = SortOption switch
            {
                "Newest" => Books.OrderByDescending(b => b.PublicationYear).ToList(),
                "Alphabetical" => Books.OrderBy(b => b.Title).ToList(),
                "Most Popular" => Books.OrderByDescending(b => b.TotalCopies - b.AvailableCopies).ToList(),
                _ => Books.ToList()
            };
            
            Books.Clear();
            foreach (var book in sortedList)
            {
                Books.Add(book);
            }
            
            // Also apply the same sorting to the filtered books
            ApplySortingToFiltered();
        }
        
        private void ApplySortingToFiltered()
        {
            if (FilteredBooks.Count == 0) return;
            
            var sortedList = SortOption switch
            {
                "Newest" => FilteredBooks.OrderByDescending(b => b.PublicationYear).ToList(),
                "Alphabetical" => FilteredBooks.OrderBy(b => b.Title).ToList(),
                "Most Popular" => FilteredBooks.OrderByDescending(b => b.TotalCopies - b.AvailableCopies).ToList(),
                _ => FilteredBooks.ToList()
            };
            
            FilteredBooks.Clear();
            foreach (var book in sortedList)
            {
                FilteredBooks.Add(book);
            }
        }

        private async Task BorrowBookAsync(Book book)
        {
            if (book == null || !_authService.IsAuthenticated) return;

            try
            {
                bool success = await _borrowingService.BorrowBookAsync(book.Id, _authService.CurrentUser.Id);
                if (success)
                {
                    // Update the book in the list
                    await SearchBooksAsync();
                    // Show success notification
                    _notificationService.ShowSuccess($"Successfully borrowed \"{book.Title}\"");
                }
                else
                {
                    // Show error notification
                    _notificationService.ShowError("Failed to borrow book. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Error borrowing book");
                _notificationService.ShowError("An error occurred while borrowing the book.");
            }
        }

        private async Task ReserveBookAsync(Book book)
        {
            if (book == null || !_authService.IsAuthenticated) return;

            try
            {
                bool success = await _borrowingService.ReserveBookAsync(book.Id, _authService.CurrentUser.Id);
                if (success)
                {
                    // Show success notification
                    _notificationService.ShowSuccess($"Successfully reserved \"{book.Title}\"");
                }
                else
                {
                    // Show error notification
                    _notificationService.ShowError("Failed to reserve book. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Error reserving book");
                _notificationService.ShowError("An error occurred while reserving the book.");
            }
        }

        private void ViewBookDetails(Book book)
        {
            if (book == null) return;
            SelectedBook = book;
            
            // Navigate to the BookDetailsPage using the navigation service
            _navigationService.NavigateTo<Views.Pages.BookDetailsPage>(book);
        }

        private void ClearFilters()
        {
            SearchTerm = string.Empty;
            SelectedCategory = "All Categories";
            SelectedAcademicLevel = "All Levels";
            AvailabilityFilter = null;
            
            // Reset category filters
            foreach (var key in CategoryFilters.Keys.ToList())
            {
                CategoryFilters[key] = false;
            }
            
            // Reset language filters
            foreach (var key in LanguageFilters.Keys.ToList())
            {
                LanguageFilters[key] = false;
            }
            
            // Trigger search with cleared filters
            SearchCommand.Execute(null);
        }

        private bool CanBorrowBook(object bookObj)
        {
            if (bookObj is Book book)
            {
                return _authService.IsAuthenticated && book.IsAvailable;
            }
            return false;
        }

        private bool CanReserveBook(object bookObj)
        {
            if (bookObj is Book book)
            {
                return _authService.IsAuthenticated && !book.IsAvailable;
            }
            return false;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
} 