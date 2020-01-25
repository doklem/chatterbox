using Chatterbox.Client.Cross.Abstractions;
using Chatterbox.Client.DataAccess.Abstractions;
using Chatterbox.Contracts.Messages;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Chatterbox.Client.ViewModels
{
    /// <summary>
    /// This class represents the <see cref="Views.ChatControl"/>'s view model. It follows the MVVM pattern.
    /// </summary>
    public class ChatViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        /// Gets the name of the <see cref="ConnectionState"/>-property.
        /// </summary>
        private static readonly string connectionStatePropertyName = nameof(ConnectionState);

        /// <summary>
        /// Gets the <see cref="IChatClient"/> which will be used to communicate with the server's hub.
        /// </summary>
        private readonly IChatClient client;

        /// <summary>
        /// Gets the <see cref="IDispatcher"/> which encapsulates the access to the <see cref="System.Windows.Threading.Dispatcher"/>.
        /// </summary>
        private readonly IDispatcher dispatcher;

        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used by this class for writing log messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        private readonly string userName;

        /// <summary>
        /// The actual field behind the <see cref="NewMessage"/> property.
        /// </summary>
        private string newMessage;

        /// <summary>
        /// Gets the state of the connection to the server.
        /// </summary>
        public ConnectionState ConnectionState { get { return client.State; } }

        /// <summary>
        /// Gets all <see cref="ChatMessage"/>s, which where sent by the server.
        /// </summary>
        public ObservableCollection<MessageListItemBase> Messages { get; private set; }

        /// <summary>
        /// Gets or sets the text of a new message. Changing it will trigger a <see cref="ObservableObject.PropertyChanged"/> event.
        /// </summary>
        public string NewMessage
        {
            get { return newMessage; }

            set
            {
                if (!string.Equals(newMessage, value))
                {
                    newMessage = value;
                    logger.LogDebug("Raising property changed event for {property}", "NewMessage");
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="IAsyncCommand"/>, which will send a new message to the server.
        /// </summary>
        public IAsyncCommand SendCommand { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ChatViewModel"/>.
        /// </summary>
        /// <param name="client">This instance of <see cref="IChatClient"/> will become the view model's <see cref="client"/>.</param>
        /// <param name="dispatcher">This instance of <see cref="IDispatcher"/> will become the view model's <see cref="dispatcher"/>.</param>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="ChatViewModel"/>'s <see cref="logger"/>.</param>
        /// <param name="sendCommand">This instance of <see cref="AsyncCommand"/> will become the <see cref="ChatViewModel"/>'s <see cref="SendCommand"/>.</param>
        /// <param name="session">This <see cref="IUserSession"/>'s <see cref="IUserSession.UserName"/> will be used as sender for new messages.</param>
        public ChatViewModel(IChatClient client, IDispatcher dispatcher, ILogger<ChatViewModel> logger, IAsyncCommand sendCommand, IUserSession session)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            userName = session.UserName;
            client.ObtainMessage += OnObtainMessage;
            client.StateChangedAsync += OnConnectedChangedAsync;
            Messages = new ObservableCollection<MessageListItemBase>(client.Select(ToMessageListItem));
            SendCommand = sendCommand ?? throw new ArgumentNullException(nameof(sendCommand));
            SendCommand.CanExecuteFunc = CanSendMessage;
            SendCommand.ExecuteFunc = SendMessageAsync;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            logger.LogDebug("Disposing");
            client.ObtainMessage -= OnObtainMessage;
            client.StateChangedAsync -= OnConnectedChangedAsync;
            logger.LogDebug("Disposed");
        }

        /// <summary>
        /// Tells if its possible to send messages at the moment or not.
        /// </summary>
        /// <returns>If sending is possible, it will return <code>true</code> else <code>false</code>.</returns>
        private bool CanSendMessage()
        {
            return client.State == ConnectionState.Connected;
        }

        /// <summary>
        /// Handles the changes of the <see cref="IChatClient"/>'s connectivity.
        /// </summary>
        private async Task OnConnectedChangedAsync()
        {
            await dispatcher.InvokeAsync(() =>
            {
                logger.LogDebug("Raising property changed event for {property}", connectionStatePropertyName);
                RaisePropertyChanged(connectionStatePropertyName);
                SendCommand.RaiseCanExecuteChanged();
            });
        }

        /// <summary>
        /// Handels new <see cref="ChatMessage"/>s, which do come in from the server.
        /// </summary>
        /// <param name="message">This instance of <see cref="MessageListItemBase"/> will be added to the <see cref="Messages"/>.</param>
        private void OnObtainMessage(ChatMessage message)
        {
            logger.LogDebug("Obtained the following text {message} from {sender}", message.Text, message.Sender);
            dispatcher.Invoke(() => Messages.Add(ToMessageListItem(message)));
        }

        /// <summary>
        /// Sends a new message to the server.
        /// </summary>
        private async Task SendMessageAsync()
        {
            try
            {
                logger.LogDebug("Sending the following text {message}", NewMessage);
                await client.SendMessageAsync(NewMessage, userName).ConfigureAwait(false);

                // Only remove the text, if the message was successfully sent by the client.
                NewMessage = string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send a message");
                throw;
            }
        }

        /// <summary>
        /// Creates a <see cref="MessageListItemBase"/> from the given <see cref="ChatMessage"/>.
        /// </summary>
        /// <param name="chatMessage">The values of the <see cref="ChatMessage"/> will be copied into the new <see cref="MessageListItemBase"/>.</param>
        /// <returns>If the given <paramref name="chatMessage"/>'s <see cref="ChatMessage.Sender"/> is equal to the <see cref="userName"/>,
        /// it will return a <see cref="OwnMessageListItem"/> else a <see cref="ForeignMessageListItem"/>.</returns>
        private MessageListItemBase ToMessageListItem(ChatMessage chatMessage)
        {
            // Check if the message was sent by this client or another.
            if (string.Equals(chatMessage.Sender, userName, StringComparison.OrdinalIgnoreCase))
            {
                return new OwnMessageListItem(chatMessage.Time.ToLocalTime().ToShortTimeString(), chatMessage.Text, chatMessage.Time);
            }
            return new ForeignMessageListItem($"{chatMessage.Time.ToLocalTime().ToShortTimeString()} {chatMessage.Sender}", chatMessage.Text, chatMessage.Time);
        }
    }
}
