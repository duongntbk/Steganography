using System;
using System.Globalization;
using System.Windows.Data;

namespace Steganography
{
    /// <summary>
    /// Converter between boolean value and EncryptionOption.
    /// Used to convert between value of radio button on UI and Encryption Option enum.
    /// </summary>
    public class EncryptionOptionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}