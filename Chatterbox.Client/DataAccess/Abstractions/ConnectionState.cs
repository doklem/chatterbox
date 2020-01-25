namespace Chatterbox.Client.DataAccess.Abstractions
{
    /// <summary>
    /// This enum wraps the <see cref="Microsoft.AspNetCore.SignalR.Client.HubConnectionState"/> of the data access layer from the UI logic.
    /// </summary>
    public enum ConnectionState
    {
        Disconnected,
        Connected,
        Connecting,
        Reconnecting
    }
}
