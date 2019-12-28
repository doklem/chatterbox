using System;

namespace Chatterbox.Client.ViewModels
{
    /// <summary>
    /// This is abstract base class the view model for a single <see cref="Contracts.ChatMessage"/>.
    /// </summary>
    public abstract class MessageListItemBase
    {
        /// <summary>
        /// Gets the message's header.
        /// </summary>
        public string Header { get; private set; }

        /// <summary>
        /// Gets the text of a message.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the text of a message. Its needed for sorting the various <see cref="Contracts.ChatMessage"/>s.
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="MessageListItemBase"/>.
        /// </summary>
        /// <param name="header">This <see cref="string"/> value will become the view model's <see cref="Header"/>.</param>
        /// <param name="text">This <see cref="string"/> value will become the view model's <see cref="Text"/>.</param>
        /// <param name="time">This <see cref="DateTime"/> value will become the view model's <see cref="Time"/>.</param>
        public MessageListItemBase(string header, string text, DateTime time)
        {
            Header = header;
            Text = text;
            Time = time;
        }
    }
}
