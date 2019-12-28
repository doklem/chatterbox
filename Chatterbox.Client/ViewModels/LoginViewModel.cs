using Chatterbox.Client.DataAccess;
using Chatterbox.Client.Helpers;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.ViewModels
{
    /// <summary>
    /// This class represents the <see cref="Views.LoginControl"/>'s view model. It follows the MVVM pattern.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used by this class for writing log messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the user's session.
        /// </summary>
        private readonly IUserSession session;

        /// <summary>
        /// Gets or sets the actual field behind the <see cref="UserName"/>-property.
        /// </summary>
        private string userName;

        /// <summary>
        /// Gets the <see cref="IAsyncCommand"/>, which will save the name of the current user.
        /// </summary>
        public IAsyncCommand LoginCommand { get; private set; }

        /// <summary>
        /// Gets or sets the name of the current user.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                logger.LogDebug("Raising property changed event for {property}", "UserName");
                RaisePropertyChanged();
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="LoginViewModel"/>.
        /// </summary>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="LoginViewModel"/>'s <see cref="logger"/>.</param>
        /// <param name="loginCommand">This instance of <see cref="AsyncCommand"/> will become the <see cref="LoginViewModel"/>'s <see cref="LoginCommand"/>.</param>
        /// <param name="session">This instance of <see cref="IUserSession"/> will become the view model's <see cref="session"/>.</param>
        public LoginViewModel(ILogger<LoginViewModel> logger, IAsyncCommand loginCommand, IUserSession session)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            LoginCommand = loginCommand ?? throw new ArgumentNullException(nameof(loginCommand));
            LoginCommand.CanExecuteFunc = CanLogin;
            LoginCommand.ExecuteFunc = LoginAsync;
            UserName = session.UserName;
        }

        /// <summary>
        /// Tells if its possible to save the name of the current user or not.
        /// </summary>
        /// <returns>If saving is possible, it returns <code>true</code> else <code>false</code>.</returns>
        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(userName);
        }

        /// <summary>
        /// Saves the name of the current user by appling it to the <see cref="session"/>.
        /// </summary>
        private async Task LoginAsync()
        {
            logger.LogDebug("Attempting to login in with the following {userName}", userName);
            await session.LoginAsync(userName);
        }
    }
}
