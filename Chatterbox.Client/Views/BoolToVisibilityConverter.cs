using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chatterbox.Client.Views
{
    /// <summary>
    /// Converts a <see cref="bool"/> into a <see cref="Visibility"/> value and backwards.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets the source's <see cref="Type"/>.
        /// </summary>
        private static readonly Type supportedBackType = typeof(bool);

        /// <summary>
        /// Gets the destination's <see cref="Type"/>.
        /// </summary>
        private static readonly Type supportedTargetType = typeof(Visibility);

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != supportedTargetType)
            {
                throw new ArgumentException(nameof(targetType));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is bool)
            {
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }
            throw new ArgumentException(nameof(value));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != supportedBackType)
            {
                throw new ArgumentException(nameof(targetType));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Visible ? true : false;
            }
            throw new ArgumentException(nameof(value));
        }
    }
}
