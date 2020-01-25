using Chatterbox.Contracts.Hubs;
using Chatterbox.Contracts.Messages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Chatterbox.Server
{
    /// <summary>
    /// This is the <see cref="Hub"/> for the chat application.
    /// </summary>
    internal class ChatHub : Hub<IChat>
    {
        /// <summary>
        /// Gets the history over all <see cref="ChatMessage"/>s.
        /// </summary>
        private readonly ConcurrentQueue<ChatMessage> history;

        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used for logging the sent messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Creates a new instance of <see cref="ChatHub"/>.
        /// </summary>
        /// <param name="history">This instance of <see cref="ConcurrentQueue{T}"/> will become the hub's <see cref="history"/>.</param>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="logger"/> of the instance.</param>
        public ChatHub(ConcurrentQueue<ChatMessage> history, ILogger<ChatHub> logger)
        {
            this.history = history ?? throw new ArgumentNullException(nameof(history));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new <see cref="ChatMessage"/> with the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">This <see cref="string"/> will become the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Text"/>.</param>
        /// <param name="sender">This <see cref="string"/> will become the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Sender"/>.</param>
        public async Task SendMessageAsync(string message, string sender)
        {
            var chatMessage = new ChatMessage
            {
                Id = Guid.NewGuid(),
                Sender = sender ?? throw new ArgumentNullException(nameof(sender)),
                Text = message ?? throw new ArgumentNullException(nameof(message)),
                Time = DateTime.UtcNow
            };
            history.Enqueue(chatMessage);
            logger.LogInformation("Sending the following message from {sender} to all clients: {message}", sender, message);
            await Clients.All.ObtainMessageAsync(chatMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the history over all <see cref="ChatMessage"/>s back to the caller.
        /// </summary>
        public async Task GetHistoryAsync()
        {
            await Clients.Caller.ObtainHistoryAsync(history).ConfigureAwait(false);
        }
    }
}
