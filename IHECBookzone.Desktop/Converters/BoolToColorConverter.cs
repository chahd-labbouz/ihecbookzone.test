using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IHECBookzone.Desktop.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string paramString)
                {
                    string[] colorValues = paramString.Split(',');
                    if (colorValues.Length == 2)
                    {
                        string colorValue = boolValue ? colorValues[0] : colorValues[1];
                        try
                        {
                            return (SolidColorBrush)new BrushConverter().ConvertFromString(colorValue);
                        }
                        catch
                        {
                            return new SolidColorBrush(Colors.Black);
                        }
                    }
                }
                
                return boolValue ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 