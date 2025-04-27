using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace IHECBookzone.Desktop.Utils
{
    public class NotificationService
    {
        private static NotificationService _instance;
        private readonly DispatcherTimer _cleanupTimer;
        private readonly int _maxNotifications = 5;
        
        public ObservableCollection<Notification> Notifications { get; } = new ObservableCollection<Notification>();
        
        public event EventHandler NotificationsChanged;

        // Singleton instance
        public static NotificationService Instance => _instance ??= new NotificationService();

        private NotificationService()
        {
            // Initialize logger event subscription
            Logger.LogMessageAdded += OnLogMessageAdded;
            
            // Initialize cleanup timer
            _cleanupTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _cleanupTimer.Tick += CleanupExpiredNotifications;
            _cleanupTimer.Start();
        }

        private void OnLogMessageAdded(object sender, LogMessageEventArgs e)
        {
            // Map log level to notification type
            NotificationType type = e.Level switch
            {
                LogLevel.Error => NotificationType.Error,
                LogLevel.Critical => NotificationType.Error,
                LogLevel.Warning => NotificationType.Warning,
                _ => NotificationType.Info
            };
            
            // Don't show debug messages as notifications
            if (e.Level == LogLevel.Debug)
                return;
                
            // Show notification for warning and above
            if (e.Level >= LogLevel.Warning)
            {
                Show(e.Message, type);
            }
        }

        /// <summary>
        /// Shows a notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="type">The notification type</param>
        /// <param name="title">Optional title</param>
        /// <param name="duration">Optional custom duration</param>
        public void Show(string message, NotificationType type = NotificationType.Info, string title = "", TimeSpan? duration = null)
        {
            if (string.IsNullOrEmpty(message))
                return;
                
            // Determine duration based on type if not specified
            TimeSpan actualDuration = duration ?? type switch
            {
                NotificationType.Error => TimeSpan.FromSeconds(8),
                NotificationType.Warning => TimeSpan.FromSeconds(5),
                _ => TimeSpan.FromSeconds(3)
            };
            
            var notification = new Notification(message, type, title)
            {
                Duration = actualDuration
            };
            
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                // Add to front
                Notifications.Insert(0, notification);
                
                // Trim if exceeded max
                while (Notifications.Count > _maxNotifications)
                {
                    Notifications.RemoveAt(Notifications.Count - 1);
                }
                
                NotificationsChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Shows a success notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        /// <param name="duration">Optional custom duration</param>
        public void ShowSuccess(string message, string title = "Success", TimeSpan? duration = null)
        {
            Show(message, NotificationType.Success, title, duration);
        }

        /// <summary>
        /// Shows an info notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        /// <param name="duration">Optional custom duration</param>
        public void ShowInfo(string message, string title = "Information", TimeSpan? duration = null)
        {
            Show(message, NotificationType.Info, title, duration);
        }

        /// <summary>
        /// Shows a warning notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        /// <param name="duration">Optional custom duration</param>
        public void ShowWarning(string message, string title = "Warning", TimeSpan? duration = null)
        {
            Show(message, NotificationType.Warning, title, duration);
        }

        /// <summary>
        /// Shows an error notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        /// <param name="duration">Optional custom duration</param>
        public void ShowError(string message, string title = "Error", TimeSpan? duration = null)
        {
            Show(message, NotificationType.Error, title, duration);
        }
        
        /// <summary>
        /// Shows a notification asynchronously
        /// </summary>
        public async Task ShowAsync(string message, NotificationType type, string title = "", TimeSpan? duration = null)
        {
            Show(message, type, title, duration);
            await Task.Delay(50); // Small delay to ensure UI updates
        }

        /// <summary>
        /// Removes a notification from the collection
        /// </summary>
        public void RemoveNotification(Notification notification)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                if (Notifications.Contains(notification))
                {
                    notification.IsVisible = false;
                    
                    // Create a delay before actually removing to allow for animation
                    Task.Delay(500).ContinueWith(_ =>
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            if (Notifications.Contains(notification))
                            {
                                Notifications.Remove(notification);
                                NotificationsChanged?.Invoke(this, EventArgs.Empty);
                            }
                        });
                    });
                }
            });
        }
        
        /// <summary>
        /// Clears all notifications from the collection
        /// </summary>
        public void ClearNotifications()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                Notifications.Clear();
                NotificationsChanged?.Invoke(this, EventArgs.Empty);
            });
        }
        
        /// <summary>
        /// Event raised when notification center visibility should be toggled
        /// </summary>
        public event EventHandler ToggleNotificationCenterRequested;
        
        /// <summary>
        /// Toggles the notification center visibility
        /// </summary>
        public void ToggleNotificationCenter()
        {
            ToggleNotificationCenterRequested?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Opens the notification center
        /// </summary>
        public void OpenNotificationCenter()
        {
            ToggleNotificationCenterRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles automatic cleanup of notifications based on lifetime
        /// </summary>
        private void CleanupExpiredNotifications(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            bool removed = false;
            
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                for (int i = Notifications.Count - 1; i >= 0; i--)
                {
                    var notification = Notifications[i];
                    if (now - notification.Timestamp > notification.Duration)
                    {
                        notification.IsVisible = false;
                        
                        // Create a delay before actually removing to allow for animation
                        int index = i; // Capture index for async operation
                        Task.Delay(500).ContinueWith(_ =>
                        {
                            Application.Current?.Dispatcher?.Invoke(() =>
                            {
                                if (index < Notifications.Count)
                                {
                                    Notifications.RemoveAt(index);
                                    removed = true;
                                }
                            });
                        });
                    }
                }
                
                if (removed)
                {
                    NotificationsChanged?.Invoke(this, EventArgs.Empty);
                }
            });
        }
    }
} 