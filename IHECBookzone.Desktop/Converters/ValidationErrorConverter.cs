using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IHECBookzone.Desktop.Converters
{
    public class ValidationErrorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // First value should be the ValidationErrors dictionary
            // Second value should be the property name to check
            
            if (values.Length < 2 || 
                !(values[0] is Dictionary<string, string> errors) || 
                !(values[1] is string propertyName))
            {
                return string.Empty;
            }
            
            // Check if there's an error for this property
            if (errors.TryGetValue(propertyName, out string errorMessage))
            {
                return errorMessage;
            }
            
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValidationErrorVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // First value should be the ValidationErrors dictionary
            // Second value should be the property name to check
            
            if (values.Length < 2 || 
                !(values[0] is Dictionary<string, string> errors) || 
                !(values[1] is string propertyName))
            {
                return Visibility.Collapsed;
            }
            
            // Check if there's an error for this property
            return errors.ContainsKey(propertyName) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 