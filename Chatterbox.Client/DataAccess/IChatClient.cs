using Chatterbox.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatterbox.Client.DataAccess
{
    /// <summary>
    /// The implementation of this interface should wrap the interaction with the server.
    /// </summary>
    public interface IChatClient : IReadOnlyCollection<ChatMessage>
    {
        /// <summary>
        /// Gets the connection state.
        /// </summary>
        ConnectionState State { get; }

        /// <summary>
        /// This event gets triggered every time the connectivity of the client does change.
        /// </summary>
        event Func<Task> StateChangedAsync;

        /// <summary>
        /// This event gets triggered every time a new message is sent by the server.
        /// </summary>
        event Action<ChatMessage> ObtainMessage;

        /// <summary>
        /// Sends a new message to the server.
        /// </summary>
        /// <param name="message">The text of the message.</param>
        /// <param name="sender">The name of the message's sender.</param>
        Task SendMessageAsync(string message, string sender);
    }
}
