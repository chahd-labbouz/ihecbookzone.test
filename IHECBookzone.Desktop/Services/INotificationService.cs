using System;
using System.Collections.ObjectModel;
using IHECBookzone.Desktop.Utils;

namespace IHECBookzone.Desktop.Services
{
    public interface INotificationService
    {
        ObservableCollection<Notification> Notifications { get; }
        event EventHandler NotificationsChanged;

        void Show(string message, NotificationType type = NotificationType.Info, string title = "");
        void ShowSuccess(string message, string title = "Success");
        void ShowError(string message, string title = "Error");
        void ShowWarning(string message, string title = "Warning");
        void ShowInfo(string message, string title = "Information");
        void Remove(Notification notification);
        void RemoveNotification(Notification notification);
        void ClearNotifications();
        void ToggleNotificationCenter();
        void OpenNotificationCenter();
    }
} 