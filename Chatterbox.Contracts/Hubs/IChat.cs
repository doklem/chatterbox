using Chatterbox.Contracts.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatterbox.Contracts.Hubs
{
    /// <summary>
    /// Represents the API of the chat SignalR hub.
    /// </summary>
    public interface IChat
    {
        /// <summary>
        /// Requests the history of <see cref="ChatMessage"/>s.
        /// </summary>
        Task GetHistoryAsync();

        /// <summary>
        /// Returns the history of <see cref="ChatMessage"/> to the caller of the <see cref="GetHistoryAsync"/>-method.
        /// </summary>
        /// <param name="history">This <see cref="IEnumerable{T}"/> contains all <see cref="ChatMessage"/>, which were sent by the server until now.</param>
        Task ObtainHistoryAsync(IEnumerable<ChatMessage> history);

        /// <summary>
        /// Each client of the chat will obtain new <see cref="ChatMessage"/> through this method.
        /// </summary>
        /// <param name="message">This <see cref="ChatMessage"/> represents the new message, which was sent by a member of the chat.</param>
        Task ObtainMessageAsync(ChatMessage message);

        /// <summary>
        /// The clients of the chat can send new <see cref="ChatMessage"/> through this method.
        /// </summary>
        /// <param name="message">This <see cref="string"/> will become the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Text"/>.</param>
        /// <param name="sender">This <see cref="string"/> will become the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Sender"/>.</param>
        Task SendMessageAsync(string message, string sender);
    }
}
