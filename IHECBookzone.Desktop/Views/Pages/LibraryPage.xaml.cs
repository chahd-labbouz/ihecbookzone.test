using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.ViewModels;

namespace IHECBookzone.Desktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        private readonly LibraryViewModel _viewModel;

        // Default constructor for design-time preview
        public LibraryPage()
        {
            InitializeComponent();
        }

        // Constructor with required services
        public LibraryPage(BookService bookService, AuthService authService, INavigationService navigationService)
        {
            if (bookService == null) throw new ArgumentNullException(nameof(bookService));
            if (authService == null) throw new ArgumentNullException(nameof(authService));
            if (navigationService == null) throw new ArgumentNullException(nameof(navigationService));

            InitializeComponent();
            
            // Create the view model with services
            _viewModel = new LibraryViewModel(bookService, authService, navigationService);
            DataContext = _viewModel;
        }

        // Constructor with existing view model
        public LibraryPage(LibraryViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _viewModel.SearchTerm = textBox.Text;
            }
        }

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _viewModel.SortOption = selectedItem.Content as string;
            }
        }

        private void CategoryCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Content is string category)
            {
                if (_viewModel.CategoryFilters.ContainsKey(category))
                {
                    _viewModel.CategoryFilters[category] = true;
                }
            }
        }
        
        private void CategoryCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Content is string category)
            {
                if (_viewModel.CategoryFilters.ContainsKey(category))
                {
                    _viewModel.CategoryFilters[category] = false;
                }
            }
        }
        
        private void LanguageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Content is string language)
            {
                if (_viewModel.LanguageFilters.ContainsKey(language))
                {
                    _viewModel.LanguageFilters[language] = true;
                }
            }
        }
        
        private void LanguageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Content is string language)
            {
                if (_viewModel.LanguageFilters.ContainsKey(language))
                {
                    _viewModel.LanguageFilters[language] = false;
                }
            }
        }
        
        private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ApplyFiltersCommand.Execute(null);
        }
        
        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearFiltersCommand.Execute(null);
        }
        
        private void BookCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Book book)
            {
                _viewModel.ViewDetailsCommand.Execute(book);
            }
        }
        
        private void BorrowButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Book book)
            {
                _viewModel.BorrowCommand.Execute(book);
            }
        }
        
        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Book book)
            {
                _viewModel.ReserveCommand.Execute(book);
            }
        }
        
        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Book book)
            {
                _viewModel.ViewDetailsCommand.Execute(book);
            }
        }
    }
} 