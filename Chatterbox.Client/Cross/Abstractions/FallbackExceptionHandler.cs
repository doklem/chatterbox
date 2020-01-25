using System;
using System.Windows;

namespace Chatterbox.Client.Cross.Abstractions
{
    /// <summary>
    /// A static handler for <see cref="Exception"/>s. It is only inteded as a fallback scenario.
    /// </summary>
    public static class FallbackExceptionHandler
    {
        /// <summary>
        /// Displays the <see cref="Exception.Message"/> of the given <see cref="Exception"/> in a <see cref="MessageBox"/>.
        /// This way of dealing with <see cref="Exception"/>s is considered as the last line of defense.
        /// </summary>
        /// <param name="ex">The <see cref="Exception.Message"/> of this <see cref="Exception"/> will become the <see cref="MessageBox"/>'s text.</param>
        public static void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
