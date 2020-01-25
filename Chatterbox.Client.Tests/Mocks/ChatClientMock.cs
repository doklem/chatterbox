using Chatterbox.Client.DataAccess.Abstractions;
using Chatterbox.Contracts.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.Mocks
{
    /// <summary>
    /// Represents a mockup of the <see cref="IChatClient"/>.
    /// </summary>
    internal class ChatClientMock : IChatClient, IDisposable
    {
        /// <inheritdoc/>
        public IList<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        /// <inheritdoc/>
        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

        /// <inheritdoc/>
        public int Count { get { return Messages.Count; } }

        /// <inheritdoc/>
        public event Func<Task> StateChangedAsync;

        /// <inheritdoc/>
        public event Action<ChatMessage> ObtainMessage;

        /// <inheritdoc/>
        public void Dispose()
        {
            StateChangedAsync = null;
            ObtainMessage = null;
        }

        /// <inheritdoc/>
        public IEnumerator<ChatMessage> GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        /// <inheritdoc/>
        public Task InitializeConnectionAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the <see cref="ChatClientMock"/>'s <see cref="IChatClient.State"/> and raises an according <see cref="IChatClient.StateChangedAsync"/> event.
        /// </summary>
        /// <param name="connectionState">This value will be applied to the <see cref="State"/>-property.</param>
        public async Task SetStateAsync(ConnectionState connectionState)
        {
            if (State == connectionState)
            {
                return;
            }
            State = connectionState;
            if (StateChangedAsync != null)
            {
                await StateChangedAsync();
            }
        }

        /// <inheritdoc/>
        public Task SendMessageAsync(string text, string sender)
        {
            var message = new ChatMessage { Id = Guid.NewGuid(), Sender = sender, Text = text, Time = DateTime.UtcNow };
            Messages.Add(message);
            ObtainMessage?.Invoke(message);
            return Task.CompletedTask;
        }
    }
}
