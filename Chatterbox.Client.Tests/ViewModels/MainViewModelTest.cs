using Chatterbox.Client.Cross.Abstractions;
using Chatterbox.Client.Cross.Implementations;
using Chatterbox.Client.DataAccess.Abstractions;
using Chatterbox.Client.Tests.Mocks;
using Chatterbox.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.ViewModels
{
    /// <summary>
    /// This is the unit test class for the <see cref="MainViewModel"/>.
    /// </summary>
    public class MainViewModelTest : TestBase
    {
        /// <summary>
        /// Tests if changes on the <see cref="MainViewModel.IsAuthenticated"/>-property will raise a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [Test]
        public async Task IsAuthenticatedPropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var userSession = provider.GetRequiredService<IUserSession>();
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised |= string.Equals(e.PropertyName, nameof(viewModel.IsAuthenticated));
                await userSession.LoginAsync("test user").ConfigureAwait(false);
                Assert.IsTrue(raised);
                raised = false;
                await userSession.LogoutAsync().ConfigureAwait(false);
                Assert.IsTrue(raised);
            }
            catch
            {
                throw;
            }
            finally
            {
                await provider.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Tests if the <see cref="MainViewModel.LogoutCommand"/>-command will trigger a logout on the <see cref="IUserSession"/>.
        /// </summary>
        [Test]
        public async Task LogoutCommand()
        {
            var provider = CreateServiceProvider();
            try
            {
                var userSession = provider.GetRequiredService<IUserSession>();
                var userName = "test user";
                await userSession.LoginAsync(userName).ConfigureAwait(false);
                var viewModel = provider.GetRequiredService<MainViewModel>();
                await viewModel.LogoutCommand.ExecuteAsync().ConfigureAwait(false);
                Assert.IsFalse(userSession.IsAuthenticated);
            }
            catch
            {
                throw;
            }
            finally
            {
                await provider.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Tests if changes on the <see cref="MainViewModel.ContentViewModel"/>-property will raise a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [Test]
        public async Task ContentViewModelPropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var userSession = provider.GetRequiredService<IUserSession>();
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised |= string.Equals(e.PropertyName, nameof(viewModel.ContentViewModel));
                await userSession.LoginAsync("test user").ConfigureAwait(false);
                Assert.IsTrue(raised);
                raised = false;
                await userSession.LogoutAsync().ConfigureAwait(false);
                Assert.IsTrue(raised);
            }
            catch
            {
                throw;
            }
            finally
            {
                await provider.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Tests if changes on the <see cref="IUserSession.IsAuthenticated"/>-property will change the <see cref="MainViewModel.ContentViewModel"/> to the correct subtype.
        /// </summary>
        [Test]
        public async Task ContentViewModelSubtype()
        {
            var provider = CreateServiceProvider();
            try
            {
                var viewModel = provider.GetRequiredService<MainViewModel>();
                Assert.IsAssignableFrom(typeof(LoginViewModel), viewModel.ContentViewModel);
                var userSession = provider.GetRequiredService<IUserSession>();
                await userSession.LoginAsync("test user");
                Assert.IsAssignableFrom(typeof(ChatViewModel), viewModel.ContentViewModel);
                await userSession.LogoutAsync();
                Assert.IsAssignableFrom(typeof(LoginViewModel), viewModel.ContentViewModel);
            }
            catch
            {
                throw;
            }
            finally
            {
                await provider.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Tests if changes on the <see cref="MainViewModel.UserName"/>-property will raise a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [Test]
        public async Task UserNamePropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var userSession = provider.GetRequiredService<IUserSession>();
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised |= string.Equals(e.PropertyName, nameof(viewModel.UserName));
                await userSession.LoginAsync("test user").ConfigureAwait(false);
                Assert.IsTrue(raised);
                raised = false;
                await userSession.LogoutAsync().ConfigureAwait(false);
                Assert.IsTrue(raised);
            }
            catch
            {
                throw;
            }
            finally
            {
                await provider.DisposeAsync().ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAsyncCommand, AsyncCommand>()
                .AddSingleton<IDispatcher, DispatcherMock>()
                .AddSingleton<IChatClient, ChatClientMock>()
                .AddSingleton<IUserSession, UserSessionMock>()
                .AddTransient<LoginViewModel>()
                .AddTransient<ChatViewModel>()
                .AddTransient<MainViewModel>();
        }
    }
}
