using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IHECBookzone.Desktop.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Check if parameter is used to invert the logic
                if (parameter != null && bool.TryParse(parameter.ToString(), out bool invertParameter) && invertParameter)
                {
                    boolValue = !boolValue;
                }
                
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                
                // Check if parameter is used to invert the logic
                if (parameter != null && bool.TryParse(parameter.ToString(), out bool invertParameter) && invertParameter)
                {
                    result = !result;
                }
                
                return result;
            }
            
            return false;
        }
    }
} 