using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using IHECBookzone.Desktop.ViewModels;

namespace IHECBookzone.Desktop.Converters
{
    public class BoolToCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAvailable && parameter is LibraryViewModel viewModel)
            {
                return isAvailable ? viewModel.BorrowCommand : viewModel.ReserveCommand;
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 