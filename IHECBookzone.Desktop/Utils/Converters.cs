using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IHECBookzone.Desktop.Utils
{
    /// <summary>
    /// Converts a boolean value to one of two string values
    /// </summary>
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;
            
            if (value is bool b)
            {
                boolValue = b;
            }
            
            string[] parameters = (parameter as string)?.Split(',');
            if (parameters == null || parameters.Length != 2)
            {
                return boolValue ? "True" : "False";
            }
            
            return boolValue ? parameters[0] : parameters[1];
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// Converts a boolean value to one of two Style resources
    /// </summary>
    public class BooleanToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;
            
            if (value is bool b)
            {
                boolValue = b;
            }
            
            string[] parameters = (parameter as string)?.Split(',');
            if (parameters == null || parameters.Length != 2)
            {
                return null;
            }
            
            string styleName = boolValue ? parameters[0] : parameters[1];
            
            try
            {
                return Application.Current.FindResource(styleName);
            }
            catch
            {
                return null;
            }
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// Converts a boolean value to a click event handler
    /// </summary>
    public class BooleanToEventConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;
            
            if (value is bool b)
            {
                boolValue = b;
            }
            
            string[] parameters = (parameter as string)?.Split(',');
            if (parameters == null || parameters.Length != 2)
            {
                return null;
            }
            
            // We can't actually return the event handler here, 
            // so we'll just return the name that the XAML binding will use
            return boolValue ? parameters[0] : parameters[1];
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 