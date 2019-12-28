using System;
using System.Globalization;
using System.Windows.Data;

namespace Chatterbox.Client.Views
{
    /// <summary>
    /// Converts an enum value into a <see cref="string"/> and backwards.
    /// </summary>
    public class EnumToStringConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (!value.GetType().IsEnum)
            {
                throw new ArgumentException(nameof(value));
            }

            return Enum.GetName(value.GetType(), value);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!targetType.IsEnum)
            {
                throw new ArgumentException(nameof(targetType));
            }
            return Enum.Parse(targetType, value as string);
        }
    }
}
