using System;
using System.Globalization;
using System.Windows.Data;

namespace IHECBookzone.Desktop.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string paramString)
                {
                    string[] values = paramString.Split(',');
                    if (values.Length == 2)
                    {
                        return boolValue ? values[0] : values[1];
                    }
                }
                
                return boolValue ? "True" : "False";
            }
            
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string paramString)
            {
                string[] values = paramString.Split(',');
                if (values.Length == 2)
                {
                    if (stringValue.Equals(values[0], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    if (stringValue.Equals(values[1], StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }
            
            return false;
        }
    }
} 