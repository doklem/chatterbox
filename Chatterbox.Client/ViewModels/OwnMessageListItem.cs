using System;

namespace Chatterbox.Client.ViewModels
{
    /// <summary>
    /// This is the view model for a single <see cref="Contracts.ChatMessage"/>, which was sent by this client.
    /// </summary>
    public class OwnMessageListItem : MessageListItemBase
    {
        /// <inheritdoc/>
        public OwnMessageListItem(string header, string text, DateTime time) : base(header, text, time)
        {
        }
    }
}
