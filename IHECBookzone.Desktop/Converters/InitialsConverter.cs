using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace IHECBookzone.Desktop.Converters
{
    public class InitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string fullName && !string.IsNullOrWhiteSpace(fullName))
            {
                // Split the name by spaces
                var nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                // If there's only one part, return the first letter
                if (nameParts.Length == 1)
                {
                    return char.ToUpper(nameParts[0][0]).ToString();
                }
                
                // Otherwise, return the first letter of the first and last parts
                var firstInitial = char.ToUpper(nameParts.First()[0]);
                var lastInitial = char.ToUpper(nameParts.Last()[0]);
                
                return $"{firstInitial}{lastInitial}";
            }
            
            // Fallback
            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 