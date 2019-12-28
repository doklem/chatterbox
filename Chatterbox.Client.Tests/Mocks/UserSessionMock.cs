using Chatterbox.Client.DataAccess;
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
        public bool IsAuthenticated { get; set; }

        /// <inheritdoc/>
        public string UserName { get; set; }

        /// <inheritdoc/>
        public event Func<Task> IsAuthenticatedChangedAsync;

        /// <inheritdoc/>
        public void Dispose()
        {
            IsAuthenticatedChangedAsync = null;
        }

        /// <inheritdoc/>
        public Task<bool> LoginAsync(string userName)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }
    }
}
