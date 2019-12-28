using System;

namespace Chatterbox.Contracts
{
    /// <summary>
    /// Represents a message of the chat.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier of the message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the message's text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the message's reception time (UTC) of the server.
        /// </summary>
        public DateTime Time { get; set; }
    }
}
