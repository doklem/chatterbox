using Chatterbox.Client.DataAccess;
using Chatterbox.Client.Helpers;
using Chatterbox.Client.Tests.Mocks;
using Chatterbox.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.ViewModels
{
    /// <summary>
    /// This is the unit test class for the <see cref="LoginViewModel"/>.
    /// </summary>
    public class LoginViewModelTest : TestBase
    {
        /// <summary>
        /// Tests if changes on the <see cref="LoginViewModel.UserName"/>-property will raise a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [Test]
        public async Task UserNamePropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var viewModel = provider.GetRequiredService<LoginViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised = string.Equals(e.PropertyName, nameof(viewModel.UserName));
                viewModel.UserName = "Alice";
                Assert.IsTrue(raised);
                raised = false;
                viewModel.UserName = "Bob";
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
        /// Tests if the <see cref="LoginViewModel.LoginCommand"/>-command will trigger a login on the <see cref="IUserSession"/>.
        /// </summary>
        [Test]
        public async Task LoginCommand()
        {
            var provider = CreateServiceProvider();
            try
            {
                var userSessionMock = provider.GetRequiredService<UserSessionMock>();
                var userName = "test user";
                userSessionMock.UserName = userName;
                var viewModel = provider.GetRequiredService<LoginViewModel>();
                viewModel.UserName = userName;
                await viewModel.LoginCommand.ExecuteAsync().ConfigureAwait(false);
                Assert.AreEqual(userName, userSessionMock.UserName);
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
        /// Tests the <see cref="LoginViewModel.LoginCommand"/>'s <see cref="AsyncCommand.CanExecute"/>-method
        /// depending on the <see cref="LoginViewModel"/>'s <see cref="LoginViewModel.UserName"/>.
        /// </summary>
        /// <param name="userName">This <see cref="string"/> will be used as the <see cref="LoginViewModel.UserName"/>.</param>
        /// <param name="expected">This value represents the expected return value of the <see cref="AsyncCommand.CanExecute"/>-method.</param>
        [TestCase("Alice", true)]
        [TestCase(" ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public async Task LoginCommandCanExecute(string userName, bool expected)
        {
            var provider = CreateServiceProvider();
            try
            {
                var viewModel = provider.GetRequiredService<LoginViewModel>();
                viewModel.UserName = userName;
                Assert.AreEqual(expected, viewModel.LoginCommand.CanExecute());
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
                .AddSingleton<DispatcherMock>()
                .AddSingleton<IDispatcher>(provider => provider.GetRequiredService<DispatcherMock>())
                .AddSingleton<UserSessionMock>()
                .AddSingleton<IUserSession>(provider => provider.GetRequiredService<UserSessionMock>())
                .AddTransient<LoginViewModel>();
        }
    }
}
