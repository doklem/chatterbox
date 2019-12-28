using System;

namespace Chatterbox.Client.Options
{
    /// <summary>
    /// This class provides access to the client application's configuration.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the connection string to the server's chat hub.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the delay between connection retries during the application startup.
        /// </summary>
        public TimeSpan InitialRetryDelay { get; set; }
    }
}
