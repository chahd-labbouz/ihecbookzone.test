using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace IHECBookzone.Desktop.Utils
{
    public enum NotificationType
    {
        Success,
        Info,
        Warning,
        Error
    }

    public class Notification : INotifyPropertyChanged
    {
        private string _title;
        private string _message;
        private NotificationType _type;
        private DateTime _timestamp;
        private bool _isVisible;
        private string _iconClass;
        private TimeSpan _duration;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public NotificationType Type
        {
            get => _type;
            set
            {
                if (SetProperty(ref _type, value))
                {
                    UpdateIconClass();
                }
            }
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public string IconClass
        {
            get => _iconClass;
            private set => SetProperty(ref _iconClass, value);
        }

        public TimeSpan Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public string Icon => Type switch
        {
            NotificationType.Success => "✓",
            NotificationType.Info => "ℹ",
            NotificationType.Warning => "⚠",
            NotificationType.Error => "✕",
            _ => "ℹ"
        };

        public Brush Background => Type switch
        {
            NotificationType.Success => new SolidColorBrush(Color.FromRgb(232, 245, 233)), // Light green
            NotificationType.Info => new SolidColorBrush(Color.FromRgb(227, 242, 253)),    // Light blue
            NotificationType.Warning => new SolidColorBrush(Color.FromRgb(255, 243, 224)), // Light orange
            NotificationType.Error => new SolidColorBrush(Color.FromRgb(255, 235, 238)),   // Light red
            _ => new SolidColorBrush(Colors.WhiteSmoke)
        };
        
        public Brush Foreground => Type switch
        {
            NotificationType.Success => new SolidColorBrush(Color.FromRgb(46, 125, 50)),   // Dark green
            NotificationType.Info => new SolidColorBrush(Color.FromRgb(13, 71, 161)),      // Dark blue
            NotificationType.Warning => new SolidColorBrush(Color.FromRgb(230, 81, 0)),    // Dark orange
            NotificationType.Error => new SolidColorBrush(Color.FromRgb(183, 28, 28)),     // Dark red
            _ => new SolidColorBrush(Colors.Black)
        };

        public Notification()
        {
            Timestamp = DateTime.Now;
            IsVisible = true;
            UpdateIconClass();
            
            // Default duration based on type
            Duration = Type switch
            {
                NotificationType.Error => TimeSpan.FromSeconds(8),
                NotificationType.Warning => TimeSpan.FromSeconds(5),
                _ => TimeSpan.FromSeconds(3)
            };
        }

        public Notification(string message, NotificationType type = NotificationType.Info, string title = "") : this()
        {
            Message = message;
            Type = type;
            Title = title;
            
            // Update duration based on type
            Duration = Type switch
            {
                NotificationType.Error => TimeSpan.FromSeconds(8),
                NotificationType.Warning => TimeSpan.FromSeconds(5),
                _ => TimeSpan.FromSeconds(3)
            };
        }

        private void UpdateIconClass()
        {
            IconClass = Type switch
            {
                NotificationType.Info => "fa-solid fa-circle-info",
                NotificationType.Success => "fa-solid fa-circle-check",
                NotificationType.Warning => "fa-solid fa-triangle-exclamation",
                NotificationType.Error => "fa-solid fa-circle-exclamation",
                _ => "fa-solid fa-circle-info"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
} 