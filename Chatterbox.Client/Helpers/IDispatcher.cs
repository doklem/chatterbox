using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.Helpers
{
    /// <summary>
    /// This interfaces serves as an abstraction for the access to the <see cref="System.Windows.Threading.Dispatcher"/>.
    /// Since it decuples the view models for the UI, it makes unit test possible.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Invokes the given <paramref name="action"/> on the UI-thread.
        /// </summary>
        /// <param name="action">This <see cref="Action"/> will be invoked on the UI-thread.</param>
        void Invoke(Action action);

        /// <summary>
        /// The asynchronous version of <see cref="Invoke(Action)"/>.
        /// </summary>
        /// <param name="action">This <see cref="Action"/> will be invoked on the UI-thread.</param>
        Task InvokeAsync(Action action);
    }
}
