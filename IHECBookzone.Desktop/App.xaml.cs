using System;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using IHECBookzone.Desktop.Services;
using IHECBookzone.Desktop.ViewModels;
using IHECBookzone.Desktop.Views;
using IHECBookzone.Desktop.Views.Pages;
using IHECBookzone.Desktop.Utils;
using IHECBookzone.Desktop.Converters;
using System.Net.Http;
using System.Text.Json;

namespace IHECBookzone.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            try
            {
                // Log application startup
                Logger.Log(LogLevel.Info, "Application starting...");
                
                // Initialize API client and make sure it can connect
                var client = ApiClient.GetClient();
                
                // Check service availability
                _ = InitializeServiceAvailabilityAsync();
                
                // Configure dependency injection
                var services = new ServiceCollection();
                ConfigureServices(services);
                _serviceProvider = services.BuildServiceProvider();
                
                // Set up global exception handling
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                
                Logger.Log(LogLevel.Info, "Application initialized successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize application: {ex.Message}", 
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
            }
        }
        
        private async Task InitializeServiceAvailabilityAsync()
        {
            // Initialize service availability checks
            try
            {
                // First, do a quick ping to check overall connectivity
                bool isSupabaseReachable = await ApiClient.CheckApiAvailabilityAsync();
                
                if (!isSupabaseReachable)
                {
                    Logger.Log(LogLevel.Warning, "Cannot reach Supabase backend. Please check your internet connection.");
                    
                    // Show a connection error dialog but don't halt the application
                    MessageBox.Show(
                        "Cannot connect to the backend services. The application will start in offline mode with limited functionality.\n\n" +
                        "Please check your internet connection and restart the application.",
                        "Connection Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    
                    return;
                }
                
                // Check all individual services
                bool allServicesAvailable = await ServiceAvailabilityChecker.CheckAllServicesAsync();
                
                if (!allServicesAvailable)
                {
                    // Get status of each service for detailed logging
                    string serviceStatus = $"Auth: {ServiceAvailabilityChecker.IsAuthServiceAvailable}, " + 
                                          $"Book: {ServiceAvailabilityChecker.IsBookServiceAvailable}, " +
                                          $"Borrowing: {ServiceAvailabilityChecker.IsBorrowingServiceAvailable}, " +
                                          $"User: {ServiceAvailabilityChecker.IsUserServiceAvailable}";
                    
                    Logger.Log(LogLevel.Warning, $"Some services are unavailable. Status: {serviceStatus}");
                    
                    // Show warning but don't halt startup
                    MessageBox.Show(
                        "Some backend services are currently unavailable. Certain features may not work correctly.\n\n" +
                        "You can continue using the application with limited functionality.",
                        "Service Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else
                {
                    Logger.Log(LogLevel.Info, "All services are available");
                }
            }
            catch (Exception ex)
            {
                // Log but don't crash the app - we'll handle unavailability when services are used
                Logger.Log(LogLevel.Error, $"Error checking service availability: {ex.Message}");
                
                MessageBox.Show(
                    $"Error checking service availability: {ex.Message}\n\n" +
                    "The application will start, but some features may not work correctly.",
                    "Service Check Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }
        }
        
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogException(e.Exception, "Unhandled exception in dispatcher");
            
            // Handle specific service unavailability exceptions
            if (e.Exception.Message.Contains("service is not available") || 
                e.Exception.Message.Contains("Failed to connect") ||
                e.Exception.Message.Contains("The operation has timed out"))
            {
                // Get the inner exception for more details
                Exception innerEx = e.Exception;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }
                
                // Try to determine which service failed
                string serviceType = "backend service";
                if (e.Exception.Message.Contains("auth") || e.Exception.Message.Contains("Authentication"))
                {
                    serviceType = "authentication service";
                }
                else if (e.Exception.Message.Contains("book") || e.Exception.Message.Contains("Book"))
                {
                    serviceType = "book service";
                }
                else if (e.Exception.Message.Contains("borrow") || e.Exception.Message.Contains("Borrow"))
                {
                    serviceType = "borrowing service";
                }
                else if (e.Exception.Message.Contains("user") || e.Exception.Message.Contains("User"))
                {
                    serviceType = "user service";
                }
                
                MessageBox.Show(
                    $"Cannot connect to the {serviceType}. Please check your internet connection or try again later.\n\n" +
                    $"Error details: {innerEx.Message}",
                    "Service Unavailable", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }
            else if (e.Exception is ApplicationException || 
                    e.Exception is HttpRequestException ||
                    e.Exception is TaskCanceledException)
            {
                // Handle API and network exceptions
                MessageBox.Show(
                    $"A network or API error occurred: {e.Exception.Message}\n\n" +
                    "This could be due to network issues or server maintenance. Please try again later.",
                    "Network Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else if (e.Exception is JsonException || 
                    e.Exception.Message.Contains("Deserialization") || 
                    e.Exception.Message.Contains("JSON"))
            {
                // Handle serialization errors
                MessageBox.Show(
                    $"Failed to process data from the server: {e.Exception.Message}",
                    "Data Processing Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                // Handle all other errors
                MessageBox.Show(
                    $"An unexpected error occurred: {e.Exception.Message}\n\n" +
                    "Please restart the application. If the problem persists, please contact support.",
                    "Application Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
            
            e.Handled = true;
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Logger.LogException(ex, "Unhandled domain exception");
            MessageBox.Show($"A critical error occurred: {ex?.Message}",
                "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        private void TaskScheduler_UnobservedTaskException(object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
        {
            Logger.LogException(e.Exception, "Unobserved task exception");
            e.SetObserved();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register utilities - fixed to use wrappers for static classes
            services.AddSingleton<IApiClient, ApiClientWrapper>();
            services.AddSingleton<ILogger, LoggerWrapper>();
            services.AddSingleton<INotificationService, Services.NotificationService>();
            
            // Register core services
            // Important: Register both interface and concrete implementation to ensure proper injection
            services.AddSingleton<AuthService>(); // Concrete type
            services.AddSingleton<IAuthService>(sp => sp.GetRequiredService<AuthService>()); // Interface mapping
            
            services.AddSingleton<BookService>(); // Concrete type
            services.AddSingleton<IBookService>(sp => sp.GetRequiredService<BookService>()); // Interface mapping
            
            // Register NavigationService with IServiceProvider
            services.AddSingleton<NavigationService>(sp => new NavigationService(sp));
            services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());
            
            services.AddSingleton<BorrowingService>(); // Concrete type
            services.AddSingleton<IBorrowingService>(sp => sp.GetRequiredService<BorrowingService>()); // Interface mapping
            
            services.AddSingleton<UserService>(); // Concrete type
            services.AddSingleton<IUserService>(sp => sp.GetRequiredService<UserService>()); // Interface mapping

            // Register ViewModels with proper service dependencies
            services.AddTransient<LibraryViewModel>(sp => new LibraryViewModel(
                sp.GetRequiredService<BookService>(),
                sp.GetRequiredService<AuthService>(),
                sp.GetRequiredService<NavigationService>(),
                sp.GetRequiredService<INotificationService>()
            ));
            services.AddTransient<BorrowingHistoryViewModel>();
            services.AddTransient<ProfileViewModel>();
            
            // Register Pages
            services.AddTransient<LoginPage>(sp => new LoginPage(
                sp.GetRequiredService<AuthService>(),
                sp.GetRequiredService<NavigationService>()
            ));
            services.AddTransient<HomePage>(sp => new HomePage(
                sp.GetRequiredService<AuthService>(),
                sp.GetRequiredService<BookService>(),
                sp.GetRequiredService<NavigationService>(),
                sp.GetRequiredService<BorrowingService>()
            ));
            services.AddTransient<BookDetailsPage>();
            services.AddTransient<AdminDashboardPage>();
            services.AddTransient<AdminLoginPage>();
            services.AddTransient<BorrowingsPage>(sp => new BorrowingsPage(
                sp.GetRequiredService<AuthService>(),
                sp.GetRequiredService<BookService>()
            ));
            services.AddTransient<LibraryPage>(sp => new LibraryPage(
                sp.GetRequiredService<BookService>(),
                sp.GetRequiredService<AuthService>(),
                sp.GetRequiredService<NavigationService>()
            ));
            services.AddTransient<ProfilePage>();
            services.AddTransient<AccessDeniedPage>();
            services.AddTransient<RegisterPage>(sp => new RegisterPage(
                sp.GetRequiredService<AuthService>(),
                sp.GetRequiredService<NavigationService>()
            ));
            services.AddTransient<RoleSelectionPage>(sp => new RoleSelectionPage(
                sp.GetRequiredService<AuthService>(),
                sp.GetRequiredService<NavigationService>()
            ));
            
            // Register main window
            services.AddTransient<MainWindow>();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Set exception handling at application level
                AppDomain.CurrentDomain.UnhandledException += (s, args) => 
                {
                    var ex = args.ExceptionObject as Exception;
                    MessageBox.Show($"Unhandled exception: {ex?.Message}\n\nStack trace: {ex?.StackTrace}", 
                        "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                };

                // Try to create the main window
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Get the innermost exception for more detail
                var innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }

                MessageBox.Show(
                    $"Error starting application: {ex.Message}\n\n" +
                    $"Inner Exception: {innerException.Message}\n\n" +
                    $"Stack Trace: {innerException.StackTrace}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                Shutdown(-1);
            }
        }
    }

    // Converters
    public class InverseBoolToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool boolValue && !boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EqualityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NameToInitialsConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string name && !string.IsNullOrEmpty(name))
            {
                var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    // Get first character of first and last name
                    return $"{parts[0][0]}{parts[parts.Length - 1][0]}";
                }
                else if (parts.Length == 1)
                {
                    // Get first character of name
                    return parts[0][0].ToString();
                }
            }
            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToStringConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is string paramString)
            {
                var options = paramString.Split(',');
                if (options.Length == 2 && value is bool boolValue)
                {
                    return boolValue ? options[0] : options[1];
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is string paramString)
            {
                var options = paramString.Split(',');
                if (options.Length == 2 && value is bool boolValue)
                {
                    return (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString(boolValue ? options[0] : options[1]);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}