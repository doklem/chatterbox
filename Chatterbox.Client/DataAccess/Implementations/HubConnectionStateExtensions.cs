using Chatterbox.Client.DataAccess.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace Chatterbox.Client.DataAccess.Implementations
{
    /// <summary>
    /// This static class provides useful functions for the <see cref="HubConnectionState"/>.
    /// </summary>
    internal static class HubConnectionStateExtensions
    {
        /// <summary>
        /// Converts the given <see cref="HubConnectionState"/> into its corresponding <see cref="ConnectionState"/>.
        /// </summary>
        /// <param name="state">This <see cref="HubConnectionState"/>-value will determin the return value.</param>
        /// <returns>Returns the <see cref="ConnectionState"/> which corresponds to the given <paramref name="state"/>.</returns>
        internal static ConnectionState ToConnectionState(this HubConnectionState state)
        {
            return state switch
            {
                HubConnectionState.Disconnected => ConnectionState.Disconnected,
                HubConnectionState.Connected => ConnectionState.Connected,
                HubConnectionState.Connecting => ConnectionState.Connecting,
                HubConnectionState.Reconnecting => ConnectionState.Reconnecting,
                _ => throw new ArgumentOutOfRangeException(nameof(state)),
            };
        }
    }
}
