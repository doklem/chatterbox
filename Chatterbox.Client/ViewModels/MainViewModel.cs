using Chatterbox.Client.Cross.Abstractions;
using Chatterbox.Client.DataAccess.Abstractions;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.ViewModels
{
    /// <summary>
    /// This class represents the <see cref="Views.MainWindow"/>'s view model. It follows the MVVM pattern.
    /// </summary>
    public class MainViewModel : ViewModelBase, IAsyncDisposable
    {
        /// <summary>
        /// Gets the name of the <see cref="UserName"/>-property.
        /// </summary>
        private static readonly string userNamePropertyName = nameof(UserName);

        /// <summary>
        /// Gets the name of the <see cref="IsAuthenticated"/>-property.
        /// </summary>
        private static readonly string isAuthenticatedPropertyName = nameof(IsAuthenticated);

        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used by this class for writing log messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> which will be used to resolve the <see cref="ContentViewModel"/> and its scope.
        /// </summary>
        private readonly IServiceProvider rootProvider;

        /// <summary>
        /// Gets the user's session.
        /// </summary>
        private readonly IUserSession session;

        /// <summary>
        /// Gets or sets the actual field behind the <see cref="ContentViewModel"/>-property.
        /// </summary>
        private ViewModelBase contentViewModel;

        /// <summary>
        /// Gets or sets the <see cref="IServiceScope"/> of the <see cref="ContentViewModel"/>.
        /// </summary>
        private IServiceScope contentViewModelScope;

        /// <summary>
        /// Tells if the desposing of this instance was already triggered or not.
        /// </summary>
        private bool isDisposing;

        /// <summary>
        /// Gets or sets the view model of the <see cref="Views.MainWindow"/>'s content.
        /// </summary>
        public ViewModelBase ContentViewModel
        {
            get { return contentViewModel; }

            set
            {
                contentViewModel = value;
                logger.LogDebug("Raising property changed event for {property}", "ContentViewModel");
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the <see cref="IAsyncCommand"/>, which end the session of the current user.
        /// </summary>
        public IAsyncCommand LogoutCommand { get; private set; }

        /// <summary>
        /// Tells if the current user is authenticated or not.
        /// </summary>
        public bool IsAuthenticated { get { return session.IsAuthenticated; } }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        public string UserName
        {
            get { return session.UserName; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="MainViewModel"/>.
        /// </summary>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="MainViewModel"/>'s <see cref="logger"/>.</param>
        /// <param name="logoutCommand">This instance of <see cref="AsyncCommand"/> will become the <see cref="MainViewModel"/>'s <see cref="LogoutCommand"/>.</param>
        /// <param name="provider">This instance of <see cref="IServiceProvider"/> will become the view model's <see cref="rootProvider"/>.</param>
        /// <param name="session">This instance of <see cref="IUserSession"/> will become the view model's <see cref="session"/>.</param>
        public MainViewModel(ILogger<MainViewModel> logger, IAsyncCommand logoutCommand, IServiceProvider provider, IUserSession session)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            rootProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            contentViewModelScope = rootProvider.CreateScope();
            ContentViewModel = this.session.IsAuthenticated 
                ? contentViewModelScope.ServiceProvider.GetRequiredService<ChatViewModel>()
                : contentViewModelScope.ServiceProvider.GetRequiredService<LoginViewModel>() as ViewModelBase;
            this.session.IsAuthenticatedChangedAsync += OnIsAuthenticatedChangedAsync;
            LogoutCommand = logoutCommand ?? throw new ArgumentNullException(nameof(logoutCommand));
            LogoutCommand.ExecuteFunc = LogoutAsync;
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            if (isDisposing)
            {
                return;
            }
            isDisposing = true;
            logger.LogDebug("Disposing");

            // Why this cast? See https://github.com/aspnet/Extensions/issues/426
            await (contentViewModelScope as IAsyncDisposable).DisposeAsync().ConfigureAwait(false);
            session.IsAuthenticatedChangedAsync -= OnIsAuthenticatedChangedAsync;
            logger.LogDebug("Disposed");
        }

        /// <summary>
        /// Ends the current user's session.
        /// </summary>
        private async Task LogoutAsync()
        {
            logger.LogDebug("Attempting to logout");
            await session.LogoutAsync();
        }

        /// <summary>
        /// Handels changes of the <see cref="session"/>'s <see cref="IUserSession.IsAuthenticated"/>.
        /// </summary>
        private async Task OnIsAuthenticatedChangedAsync()
        {
            logger.LogDebug("Disposing current content");
            // Why this cast? See https://github.com/aspnet/Extensions/issues/426
            await (contentViewModelScope as IAsyncDisposable).DisposeAsync().ConfigureAwait(false);
            contentViewModelScope = rootProvider.CreateScope();
            // If the user is authenticated show the chat or else the login.
            if (session.IsAuthenticated)
            {
                logger.LogDebug("Switching to the chat view");
                ContentViewModel = contentViewModelScope.ServiceProvider.GetRequiredService<ChatViewModel>();
            }
            else
            {
                logger.LogDebug("Switching to the login view");
                ContentViewModel = contentViewModelScope.ServiceProvider.GetRequiredService<LoginViewModel>();
            }
            logger.LogDebug("Raising property changed event for {property}", userNamePropertyName);
            RaisePropertyChanged(userNamePropertyName);
            logger.LogDebug("Raising property changed event for {property}", isAuthenticatedPropertyName);
            RaisePropertyChanged(isAuthenticatedPropertyName);
        }
    }
}
