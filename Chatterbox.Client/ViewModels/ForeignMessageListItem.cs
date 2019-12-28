using System;

namespace Chatterbox.Client.ViewModels
{
    /// <summary>
    /// This is the view model for a single <see cref="Contracts.ChatMessage"/>, which was sent by someone else.
    /// </summary>
    public class ForeignMessageListItem : MessageListItemBase
    {
        /// <inheritdoc/>
        public ForeignMessageListItem(string header, string text, DateTime time) : base(header, text, time)
        {
        }
    }
}
