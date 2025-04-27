using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IHECBookzone.Desktop.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                bool isZero = intValue == 0;
                
                // Check if inversion is requested (typically for "No results found" message)
                bool invertLogic = parameter != null && bool.TryParse(parameter.ToString(), out bool shouldInvert) && shouldInvert;
                
                if (invertLogic)
                {
                    return isZero ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return isZero ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 