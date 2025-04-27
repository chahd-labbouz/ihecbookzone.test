using System;
using System.Windows;
using System.Windows.Controls;
using IHECBookzone.Desktop.Utils;

namespace IHECBookzone.Desktop.Controls
{
    /// <summary>
    /// Interaction logic for NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : UserControl
    {
        public static readonly DependencyProperty NotificationProperty =
            DependencyProperty.Register(
                "Notification", 
                typeof(Notification), 
                typeof(NotificationControl), 
                new PropertyMetadata(null, OnNotificationChanged));

        public Notification Notification
        {
            get => (Notification)GetValue(NotificationProperty);
            set => SetValue(NotificationProperty, value);
        }

        private static void OnNotificationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotificationControl control && e.NewValue is Notification notification)
            {
                control.DataContext = notification;
            }
        }

        public NotificationControl()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Notification != null)
            {
                Utils.NotificationService.Instance.RemoveNotification(Notification);
            }
        }
    }
} 