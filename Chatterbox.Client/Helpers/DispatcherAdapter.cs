using System;
using System.Threading.Tasks;
using System.Windows;

namespace Chatterbox.Client.Helpers
{
    /// <inheritdoc/>
    internal class DispatcherAdapter : IDispatcher
    {
        /// <inheritdoc/>
        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            Application.Current.Dispatcher.Invoke(action);
        }

        /// <inheritdoc/>
        public async Task InvokeAsync(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            await Application.Current.Dispatcher.InvokeAsync(action).Task.ConfigureAwait(false);
        }
    }
}
