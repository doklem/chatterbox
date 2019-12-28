using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace Chatterbox.Client.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    internal static class HubConnectionStateExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
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
