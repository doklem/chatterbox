using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Chatterbox.Client.Tests")]
namespace Chatterbox.Client.DataAccess
{
    /// <inheritdoc/>
    internal class UserSession : IUserSession
    {
        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used by this class for writing log messages.
        /// </summary>
        private readonly ILogger logger;

        /// <inheritdoc/>
        public bool IsAuthenticated { get; private set; }

        /// <inheritdoc/>
        public string UserName { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="UserSession"/>.
        /// </summary>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="UserSession"/>'s <see cref="logger"/>.</param>
        public UserSession(ILogger<UserSession> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public event Func<Task> IsAuthenticatedChangedAsync;

        /// <inheritdoc/>
        public void Dispose()
        {
            IsAuthenticatedChangedAsync = null;
        }

        /// <inheritdoc/>
        public async Task<bool> LoginAsync(string userName)
        {
            logger.LogDebug("Attempt to login with the following user name {userName}", userName);
            if (IsAuthenticated)
            {
                return false;
            }
            UserName = !string.IsNullOrWhiteSpace(userName) ? userName : throw new ArgumentNullException(nameof(userName));
            IsAuthenticated = true;
            logger.LogInformation("Login with the following user name {userName}", userName);
            logger.LogDebug("Raising a IsAuthenticatedChangedAsync event");
            if (IsAuthenticatedChangedAsync != null)
            {
                await IsAuthenticatedChangedAsync().ConfigureAwait(false);
            }
            return true;
        }

        /// <inheritdoc/>
        public async Task LogoutAsync()
        {
            logger.LogDebug("Attempt to logout");
            if (IsAuthenticated)
            {
                UserName = null;
                IsAuthenticated = false;
                logger.LogInformation("Logout");
                logger.LogDebug("Raising a IsAuthenticatedChangedAsync event");
                if (IsAuthenticatedChangedAsync != null)
                {
                    await IsAuthenticatedChangedAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
