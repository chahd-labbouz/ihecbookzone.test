using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IHECBookzone.Desktop.Utils
{
    public class NotificationTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string type)
            {
                switch (type.ToLower())
                {
                    case "success":
                        return new SolidColorBrush(Color.FromRgb(39, 174, 96));
                    case "error":
                        return new SolidColorBrush(Color.FromRgb(231, 76, 60));
                    case "warning":
                        return new SolidColorBrush(Color.FromRgb(243, 156, 18));
                    case "info":
                    default:
                        return new SolidColorBrush(Color.FromRgb(52, 152, 219));
                }
            }
            
            // Default to info color
            return new SolidColorBrush(Color.FromRgb(52, 152, 219));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 