using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Utils;

namespace IHECBookzone.Desktop.Controls
{
    /// <summary>
    /// Interaction logic for NotificationCenter.xaml
    /// </summary>
    public partial class NotificationCenter : UserControl, INotifyPropertyChanged
    {
        private readonly Utils.NotificationService _notificationService;
        private bool _isVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        public new bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                    Visibility = _isVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        public bool HasNotifications => _notificationService?.Notifications.Count > 0;
        public bool HasNoNotifications => !HasNotifications;

        public NotificationCenter()
        {
            InitializeComponent();
            DataContext = this;
            
            _notificationService = Utils.NotificationService.Instance;
            
            if (_notificationService != null)
            {
                NotificationsItemsControl.ItemsSource = _notificationService.Notifications;
                _notificationService.NotificationsChanged += NotificationService_NotificationsChanged;
                _notificationService.ToggleNotificationCenterRequested += (s, e) => ToggleVisibility();
            }
            
            Visibility = Visibility.Collapsed;
            IsVisible = false;
        }

        private void NotificationService_NotificationsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(HasNotifications));
            OnPropertyChanged(nameof(HasNoNotifications));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsVisible = false;
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            _notificationService?.ClearNotifications();
        }

        private void DismissNotification_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Notification notification)
            {
                _notificationService?.RemoveNotification(notification);
            }
        }

        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 