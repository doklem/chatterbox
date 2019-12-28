using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace Chatterbox.Client.DataAccess
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
            switch (state)
            {
                case HubConnectionState.Disconnected:
                    return ConnectionState.Disconnected;
                case HubConnectionState.Connected:
                    return ConnectionState.Connected;
                case HubConnectionState.Connecting:
                    return ConnectionState.Connecting;
                case HubConnectionState.Reconnecting:
                    return ConnectionState.Reconnecting;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state));
            }
        }
    }
}
