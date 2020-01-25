using Chatterbox.Contracts.Hubs;
using Chatterbox.Contracts.Messages;
using Chatterbox.Server.Business.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Chatterbox.Server.Hubs
{
    /// <summary>
    /// This is the <see cref="Hub"/> for the chat application.
    /// </summary>
    internal class ChatHub : Hub<IChat>
    {
        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used for logging the sent messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the server's lifetime, which will be used to cancell DB-operations during shutdown.
        /// </summary>
        private readonly IHostApplicationLifetime hostApplicationLifetime;

        /// <summary>
        /// Gets the <see cref="IMessageManager"/>, which responsible for handling the logic related to <see cref="ChatMessage"/>s.
        /// </summary>
        private readonly IMessageManager messageManager;

        /// <summary>
        /// Creates a new instance of <see cref="ChatHub"/>.
        /// </summary>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="logger"/> of the instance.</param>
        /// <param name="hostApplicationLifetime">This <see cref="IHostApplicationLifetime"/> will become the <see cref="hostApplicationLifetime"/> of the instance.</param>
        /// <param name="messageManager">This <see cref="IMessageManager"/> will become the <see cref="messageManager"/> of the instance.</param>
        public ChatHub(ILogger<ChatHub> logger, IHostApplicationLifetime hostApplicationLifetime, IMessageManager messageManager)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            this.messageManager = messageManager ?? throw new ArgumentNullException(nameof(messageManager));
        }

        /// <summary>
        /// Creates a new <see cref="ChatMessage"/> with the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message">This <see cref="string"/> will become the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Text"/>.</param>
        /// <param name="sender">This <see cref="string"/> will become the <see cref="ChatMessage"/>'s <see cref="ChatMessage.Sender"/>.</param>
        public async Task SendMessageAsync(string message, string sender)
        {
            var chatMessage = await messageManager.CreateAsync(sender, message, hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
            logger.LogInformation("Sending the following message from {sender} to all clients: {message}", sender, message);
            await Clients.All.ObtainMessageAsync(chatMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the history over all <see cref="ChatMessage"/>s back to the caller.
        /// </summary>
        public async Task GetHistoryAsync()
        {
            await Clients.Caller
                .ObtainHistoryAsync(await messageManager.GetAllAsync(hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
