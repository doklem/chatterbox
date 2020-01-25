using Chatterbox.Contracts.Messages;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chatterbox.Server.Business.Abstractions
{
    /// <summary>
    /// Following the manager design pattern, the implementation of this interface encapsulates the logic for <see cref="ChatMessage"/>s.
    /// </summary>
    public interface IMessageManager
    {
        /// <summary>
        /// Creates a new <see cref="ChatMessage"/> and persists it.
        /// </summary>
        /// <param name="sender">This <see cref="string"/> value will be used as the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Sender"/>.</param>
        /// <param name="text">This <see cref="string"/> value will be used as the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Text"/>.</param>
        /// <param name="cancellationToken">This optional <see cref="CancellationToken"/> could be used to abort the creation of a new <see cref="ChatMessage"/>.</param>
        /// <returns>Returns the newly created <see cref="ChatMessage"/>.</returns>
        Task<ChatMessage> CreateAsync(string sender, string text, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="ChatMessage"/>, which were sent to the server in the past.
        /// </summary>
        /// <param name="cancellationToken">This optional <see cref="CancellationToken"/> could be used to abort the query for all <see cref="ChatMessage"/>s.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}"/> over all <see cref="ChatMessage"/>, which were sent to the server in the past.</returns>
        Task<IEnumerable<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
