using Chatterbox.Client.DataAccess.Abstractions;
using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.Mocks
{
    /// <summary>
    /// Represents a mockup of the <see cref="IUserSession"/>.
    /// </summary>
    internal class UserSessionMock : IUserSession
    {
        /// <inheritdoc/>
        public bool IsAuthenticated { get; private set; }

        /// <inheritdoc/>
        public string UserName { get; private set; }

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
            UserName = userName;
            IsAuthenticated = true;
            if (IsAuthenticatedChangedAsync != null)
            {
                await IsAuthenticatedChangedAsync();
            }
            return true;
        }

        /// <inheritdoc/>
        public async Task LogoutAsync()
        {
            UserName = null;
            IsAuthenticated = false;
            if (IsAuthenticatedChangedAsync != null)
            {
                await IsAuthenticatedChangedAsync();
            }
        }
    }
}
