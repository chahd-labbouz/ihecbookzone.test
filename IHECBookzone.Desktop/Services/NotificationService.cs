using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using IHECBookzone.Desktop.Utils;

namespace IHECBookzone.Desktop.Services
{
    /// <summary>
    /// Façade for Utils.NotificationService to maintain backward compatibility
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly Utils.NotificationService _notificationService;

        public ObservableCollection<Notification> Notifications => _notificationService.Notifications;
        
        public event EventHandler NotificationsChanged;

        public static NotificationService Instance => _instance ??= new NotificationService();
        private static NotificationService _instance;

        private NotificationService()
        {
            _notificationService = Utils.NotificationService.Instance;
            _notificationService.NotificationsChanged += (s, e) => NotificationsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Shows a notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="type">The notification type</param>
        /// <param name="title">Optional title</param>
        public void Show(string message, NotificationType type = NotificationType.Info, string title = "")
        {
            _notificationService.Show(message, type, title);
        }

        /// <summary>
        /// Shows a success notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        public void ShowSuccess(string message, string title = "Success")
        {
            _notificationService.ShowSuccess(message, title);
        }

        /// <summary>
        /// Shows an error notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        public void ShowError(string message, string title = "Error")
        {
            _notificationService.ShowError(message, title);
        }

        /// <summary>
        /// Shows a warning notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        public void ShowWarning(string message, string title = "Warning")
        {
            _notificationService.ShowWarning(message, title);
        }

        /// <summary>
        /// Shows an info notification
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="title">Optional title</param>
        public void ShowInfo(string message, string title = "Information")
        {
            _notificationService.ShowInfo(message, title);
        }

        /// <summary>
        /// Removes a notification from the collection
        /// </summary>
        /// <param name="notification">The notification to remove</param>
        public void Remove(Notification notification)
        {
            _notificationService.RemoveNotification(notification);
        }
        
        /// <summary>
        /// Removes a notification from the collection (compatibility method)
        /// </summary>
        public void RemoveNotification(Notification notification)
        {
            _notificationService.RemoveNotification(notification);
        }
        
        /// <summary>
        /// Clears all notifications
        /// </summary>
        public void ClearNotifications()
        {
            _notificationService.ClearNotifications();
        }
        
        /// <summary>
        /// Toggles notification center visibility
        /// </summary>
        public void ToggleNotificationCenter()
        {
            _notificationService.ToggleNotificationCenter();
        }
        
        /// <summary>
        /// Opens the notification center
        /// </summary>
        public void OpenNotificationCenter()
        {
            _notificationService.OpenNotificationCenter();
        }
    }
} 