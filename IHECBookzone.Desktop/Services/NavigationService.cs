using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace IHECBookzone.Desktop.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _mainFrame;
        private readonly IServiceProvider _serviceProvider;

        public Frame MainFrame 
        { 
            get => _mainFrame;
            set => _mainFrame = value; 
        }

        public bool CanGoBack => _mainFrame?.CanGoBack ?? false;
        public bool CanGoForward => _mainFrame?.CanGoForward ?? false;

        // Constructor for dependency injection
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        // Default constructor for direct instantiation
        public NavigationService()
        {
            _serviceProvider = null;
        }

        public void Initialize(Frame frame)
        {
            _mainFrame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public bool NavigateTo<T>(object parameter = null) where T : Page
        {
            if (_mainFrame == null)
            {
                FindMainFrame();
            }

            if (_mainFrame != null)
            {
                try
                {
                    T page;
                    if (_serviceProvider != null)
                    {
                        // Use dependency injection if available
                        page = _serviceProvider.GetRequiredService<T>();
                    }
                    else
                    {
                        // Fallback to direct instantiation
                        page = Activator.CreateInstance<T>();
                    }

                    // Set parameters if needed
                    if (parameter != null && page is FrameworkElement element)
                    {
                        element.DataContext = parameter;
                    }

                    return _mainFrame.Navigate(page);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error navigating to page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return false;
        }

        public bool NavigateTo(Type pageType, object parameter = null)
        {
            if (_mainFrame == null)
            {
                FindMainFrame();
            }

            if (_mainFrame != null)
            {
                try
                {
                    Page page;
                    if (_serviceProvider != null)
                    {
                        // Use dependency injection if available
                        page = _serviceProvider.GetRequiredService(pageType) as Page;
                    }
                    else
                    {
                        // Fallback to direct instantiation
                        page = Activator.CreateInstance(pageType) as Page;
                    }

                    // Set parameters if needed
                    if (parameter != null && page is FrameworkElement element)
                    {
                        element.DataContext = parameter;
                    }

                    return _mainFrame.Navigate(page);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error navigating to page: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return false;
        }

        public bool NavigateToUri(Uri uri)
        {
            if (_mainFrame == null)
            {
                FindMainFrame();
            }

            if (_mainFrame != null)
            {
                return _mainFrame.Navigate(uri);
            }

            return false;
        }

        public bool GoBack()
        {
            if (_mainFrame == null)
            {
                FindMainFrame();
            }

            if (_mainFrame != null && _mainFrame.CanGoBack)
            {
                _mainFrame.GoBack();
                return true;
            }

            return false;
        }

        public bool GoForward()
        {
            if (_mainFrame == null)
            {
                FindMainFrame();
            }

            if (_mainFrame != null && _mainFrame.CanGoForward)
            {
                _mainFrame.GoForward();
                return true;
            }

            return false;
        }

        public void ClearHistory()
        {
            if (_mainFrame != null)
            {
                while (_mainFrame.CanGoBack)
                {
                    _mainFrame.RemoveBackEntry();
                }
            }
        }

        public void RefreshCurrentPage()
        {
            if (_mainFrame != null)
            {
                var current = _mainFrame.Content;
                _mainFrame.Navigate(null);
                _mainFrame.Navigate(current);
            }
        }

        public bool NavigateTo(string pageName, object parameter = null)
        {
            // Get page type based on the page name
            switch (pageName)
            {
                case "RoleSelection":
                    return NavigateTo<Views.Pages.RoleSelectionPage>(parameter);
                case "Login":
                    return NavigateTo<Views.Pages.LoginPage>(parameter);
                case "Register":
                    return NavigateTo<Views.Pages.RegisterPage>(parameter);
                case "AdminLogin":
                    return NavigateTo<Views.Pages.AdminLoginPage>(parameter);
                case "Home":
                    return NavigateTo<Views.Pages.HomePage>(parameter);
                case "Library":
                    return NavigateTo<Views.Pages.LibraryPage>(parameter);
                case "Profile":
                    return NavigateTo<Views.Pages.ProfilePage>(parameter);
                case "Admin":
                    return NavigateTo<Views.Pages.AdminDashboardPage>(parameter);
                default:
                    // If pageName is not recognized, return false to indicate failure
                    return false;
            }
        }

        private void FindMainFrame()
        {
            // Try to find the MainFrame from the current window
            var currentWindow = Application.Current.MainWindow;
            if (currentWindow != null)
            {
                _mainFrame = currentWindow.FindName("MainFrame") as Frame;
            }
        }
    }
} 