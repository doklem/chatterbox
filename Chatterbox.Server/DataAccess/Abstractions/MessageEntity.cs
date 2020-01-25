using System;
using System.ComponentModel.DataAnnotations;

namespace Chatterbox.Server.DataAccess.Abstractions
{
    /// <summary>
    /// Represents the database entity of a chat message.
    /// </summary>
    public class MessageEntity
    {
        [Key]
        /// <summary>
        /// Gets or sets the unique identifier of the message.
        /// </summary>
        public long Id { get; set; }

        [Required]
        /// <summary>
        /// Gets or sets the message's sender.
        /// </summary>
        public string Sender { get; set; }

        [Required]
        /// <summary>
        /// Gets or sets the message's text.
        /// </summary>
        public string Text { get; set; }

        [Required]
        /// <summary>
        /// Gets or sets the message's reception time (UTC) of the server.
        /// </summary>
        public DateTime Time { get; set; }
    }
}
