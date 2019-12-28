﻿using Chatterbox.Client.Helpers;
using Chatterbox.Client.Options;
using Chatterbox.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chatterbox.Client.DataAccess
{
    /// <inheritdoc/>
    internal class ChatClient : IHostedService
    {
        /// <summary>
        /// Gets the name of the SingalR hub's method for requesting the history over all <see cref="ChatMessage"/>s.
        /// </summary>
        private static readonly string GetHistoryMethodName = nameof(IChat.GetHistoryAsync);

        /// <summary>
        /// Gets the name of the SingalR hub's method for obtaining the history over all <see cref="ChatMessage"/>s.
        /// </summary>
        private static readonly string ReceiveHistoryMethodName = nameof(IChat.ObtainHistoryAsync);

        /// <summary>
        /// Gets the name of the SingalR hub's method for obtaining a new <see cref="ChatMessage"/>.
        /// </summary>
        private static readonly string ReceiveMethodName = nameof(IChat.ObtainMessageAsync);

        /// <summary>
        /// Gets the name of the SignalR hub's method for sending a new <see cref="ChatMessage"/>.
        /// </summary>
        private static readonly string SendMethodName = nameof(IChat.SendMessageAsync);

        /// <summary>
        /// Gets the <see cref="IHubConnectionBuilder"/>, which will be used to creat the connection to the server.
        /// </summary>
        private readonly IHubConnectionBuilder builder;

        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used by this class for writing log messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the history over all <see cref="ChatMessage"/>s.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, ChatMessage> messages;

        /// <summary>
        /// Gets the client's configuration.
        /// </summary>
        private readonly IOptionsMonitor<AppSettings> options;

        /// <summary>
        /// Gets the connection to the server's chat hub.
        /// </summary>
        private HubConnection connection;

        /// <summary>
        /// Gets the handel, which will be used unregister from the event of incomming histories.
        /// </summary>
        private IDisposable receiveHistoryHandel;

        /// <summary>
        /// Gets the handel, which will be used unregister from the event of incomming messages.
        /// </summary>
        private IDisposable receiveMessageHandel;

        /// <summary>
        /// Creates a new instance of <see cref="ChatClient"/>.
        /// </summary>
        /// <param name="builder">This <see cref="IHubConnectionBuilder"/> will be used to create the <see cref="HubConnection"/> to the server.</param>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="ChatClient"/>'s <see cref="logger"/>.</param>
        /// <param name="options">This <see cref="AppSettings"/> will become the <see cref="ChatClient"/>'s <see cref="options"/>.</param>
        public ChatClient(IHubConnectionBuilder builder, ILogger<ChatClient> logger, IOptionsMonitor<AppSettings> options)
        {
            this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            messages = new ConcurrentDictionary<Guid, ChatMessage>();
        }

        /// <inheritdoc/>
        public async Task SendMessageAsync(string message)
        {
            logger.LogInformation("Sending the following message '{message}'", message);
            await connection.InvokeAsync(SendMethodName, message, options.CurrentValue.Sender)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Starting");
            connection = builder.WithAutomaticReconnect()
                .WithUrl(options.CurrentValue.ConnectionString)
                .Build();
            receiveHistoryHandel = connection.On<IEnumerable<ChatMessage>>(ReceiveHistoryMethodName, OnReceiveHistory);
            receiveMessageHandel = connection.On<ChatMessage>(ReceiveMethodName, OnReceiveMessage);
            connection.Closed += OnStateChangedAsync;
            connection.Reconnected += OnStateChangedAsync;
            connection.Reconnecting += OnStateChangedAsync;

            // Don't block the application's startup by awaiting the end of the successful connection establishment.
            InitalizeConnection(cancellationToken).FireAndForgetSafeAsync(logger);
            logger.LogDebug("Started");
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Stopping");
            connection.Closed -= OnStateChangedAsync;
            connection.Reconnected -= OnStateChangedAsync;
            connection.Reconnecting -= OnStateChangedAsync;
            receiveHistoryHandel.Dispose();
            receiveMessageHandel.Dispose();
            await connection.DisposeAsync().ConfigureAwait(false);
            logger.LogDebug("Stopped");
        }

        /// <summary>
        /// Tries to establish the initial connection with the server by using an endless loop until contact is made.
        /// </summary>
        /// <param name="cancellationToken">This <see cref="CancellationToken"/> enables breaking the endless load.</param>
        private async Task InitalizeConnection(CancellationToken cancellationToken)
        {
            while (connection.State == HubConnectionState.Disconnected && !cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug("Trying to initialize the connection to the server");
                try
                {
                    await connection.StartAsync(cancellationToken).ConfigureAwait(false);
                    await OnStateChangedAsync(null).ConfigureAwait(false);
                    logger.LogInformation("Requesting the history");
                    await connection.InvokeAsync(GetHistoryMethodName, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unable to initialize connection to the server. Retrying...");
                    if (connection.State == HubConnectionState.Disconnected && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(options.CurrentValue.InitialRetryDelay, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Handels the changes of the <see cref="connection"/>'s <see cref="HubConnection.State"/>.
        /// </summary>
        private Task OnStateChangedAsync(object arg)
        {
            logger.LogDebug("Connection state changed");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handels the incomming history over <see cref="ChatMessage"/>s.
        /// </summary>
        /// <param name="history"></param>
        private void OnReceiveHistory(IEnumerable<ChatMessage> history)
        {
            logger.LogInformation("Received the history with {historySize} messages", history.Count());
            foreach (var message in history)
            {
                messages.AddOrUpdate(message.Id, message, (id, oldValue) => message);
            }

            // ToDo: Remove this, since it is only intened for development purposes.
            SendMessageAsync("Hello world").FireAndForgetSafeAsync();
        }

        /// <summary>
        /// Handels the incomming new <see cref="ChatMessage"/>.
        /// </summary>
        /// <param name="message">If this <see cref="ChatMessage"/> will be added/updated in the <see cref="messages"/>.
        /// If it's new, a <see cref="ObtainMessage"/>-event will be raised.</param>
        private void OnReceiveMessage(ChatMessage message)
        {
            logger.LogInformation("Received the message '{message}' from {sender}", message.Text, message.Sender);
            bool isNew = false;
            messages.AddOrUpdate(
                message.Id,
                id =>
                {
                    isNew = true;
                    return message;
                },
                (id, oldValue) => message);
            if (isNew)
            {
                logger.LogDebug("Obtained a new message");
            }
        }
    }
}
