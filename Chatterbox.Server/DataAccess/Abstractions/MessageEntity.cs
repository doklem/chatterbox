using System;
using System.ComponentModel.DataAnnotations;

namespace Chatterbox.Server.DataAccess.Abstractions
{
    /// <summary>
    /// Represents the database entity of a chat message.
    /// </summary>
    public class MessageEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the message.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        [Required]
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the message's text.
        /// </summary>
        [Required]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the message's reception time (UTC) of the server.
        /// </summary>
        [Required]
        public DateTime Time { get; set; }
    }
}
