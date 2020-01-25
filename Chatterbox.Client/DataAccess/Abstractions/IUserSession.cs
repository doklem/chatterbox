using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.DataAccess.Abstractions
{
    /// <summary>
    /// This is the model for a user session.
    /// </summary>
    public interface IUserSession : IDisposable
    {
        /// <summary>
        /// Gets the flag, which tells if the current user is authenticated or not.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// This event gets triggered, when the <see cref="IsAuthenticated"/>-property value does change.
        /// </summary>
        event Func<Task> IsAuthenticatedChangedAsync;

        /// <summary>
        /// Login for the user with the given name.
        /// </summary>
        /// <param name="userName">The name of the current user.</param>
        /// <returns>Returns <code>true</code> if the login attempt was successful, else <code>false</code>.</returns>
        Task<bool> LoginAsync(string userName);

        /// <summary>
        /// Logout for the current user.
        /// </summary>
        Task LogoutAsync();
    }
}
