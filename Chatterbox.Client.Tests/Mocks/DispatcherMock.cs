using Chatterbox.Client.Helpers;
using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.Mocks
{
    /// <summary>
    /// Represents a mockup of the <see cref="IDispatcher"/>.
    /// </summary>
    internal class DispatcherMock : IDispatcher
    {
        /// <inheritdoc/>
        public void Invoke(Action action)
        {
            action();
        }

        /// <inheritdoc/>
        public Task InvokeAsync(Action action)
        {
            action();
            return Task.CompletedTask;
        }
    }
}
