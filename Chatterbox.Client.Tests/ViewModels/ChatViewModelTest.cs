using Chatterbox.Client.Cross.Abstractions;
using Chatterbox.Client.Cross.Implementations;
using Chatterbox.Client.DataAccess.Abstractions;
using Chatterbox.Client.Tests.Mocks;
using Chatterbox.Client.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.ViewModels
{
    /// <summary>
    /// This is the unit test class for the <see cref="ChatViewModel"/>.
    /// </summary>
    public class ChatViewModelTest : TestBase
    {
        /// <summary>
        /// Tests if changes on the <see cref="ChatViewModel.NewMessage"/>-property will raise a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [Test]
        public async Task NewMessagePropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var viewModel = provider.GetRequiredService<ChatViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised |= string.Equals(e.PropertyName, nameof(viewModel.NewMessage));
                viewModel.NewMessage = "Hey Bob";
                Assert.IsTrue(raised);
                raised = false;
                viewModel.NewMessage = "Hey Alice";
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
        /// Tests if changes on the <see cref="IChatClient.State"/>-property will raise a
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event of the <see cref="ChatViewModel.ConnectionState"/>.
        /// </summary>
        [Test]
        public async Task ConnectionStatePropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var chatClientMock = provider.GetRequiredService<ChatClientMock>();
                var viewModel = provider.GetRequiredService<ChatViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised |= string.Equals(e.PropertyName, nameof(viewModel.ConnectionState));
                await chatClientMock.SetStateAsync(ConnectionState.Connected).ConfigureAwait(false);
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
        /// Tests if the <see cref="ChatViewModel.SendCommand"/>-command will trigger a new message on the <see cref="IChatClient"/>.
        /// </summary>
        [Test]
        public async Task SendCommand()
        {
            var provider = CreateServiceProvider();
            try
            {
                var chatClientMock = provider.GetRequiredService<ChatClientMock>();
                var userSession = provider.GetRequiredService<IUserSession>();
                await chatClientMock.SetStateAsync(ConnectionState.Connected).ConfigureAwait(false);
                var text = "test message";
                var sender = "test user";
                await userSession.LoginAsync(sender).ConfigureAwait(false);
                var viewModel = provider.GetRequiredService<ChatViewModel>();
                viewModel.NewMessage = text;
                await viewModel.SendCommand.ExecuteAsync().ConfigureAwait(false);
                var message = chatClientMock.Messages.Last();
                Assert.AreEqual(text, message.Text);
                Assert.AreEqual(sender, message.Sender);
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
        /// Tests the <see cref="ChatViewModel.SendCommand"/>'s <see cref="AsyncCommand.CanExecute"/>-method depending on
        /// the <see cref="IChatClient"/>'s <see cref="ConnectionState"/>.
        /// </summary>
        /// <param name="state">This value will be used as the <see cref="IChatClient"/>'s <see cref="ConnectionState"/>.</param>
        /// <param name="expected">This value represents the expected return value of the <see cref="AsyncCommand.CanExecute"/>-method.</param>
        [TestCase(ConnectionState.Connected, true)]
        [TestCase(ConnectionState.Connecting, false)]
        [TestCase(ConnectionState.Disconnected, false)]
        [TestCase(ConnectionState.Reconnecting, false)]
        public async Task SendCommandCanExecute(ConnectionState state, bool expected)
        {
            var provider = CreateServiceProvider();
            try
            {
                var viewModel = provider.GetRequiredService<ChatViewModel>();
                var chatClientMock = provider.GetRequiredService<ChatClientMock>();
                await chatClientMock.SetStateAsync(state).ConfigureAwait(false);
                Assert.AreEqual(expected, viewModel.SendCommand.CanExecute());
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
                .AddSingleton<ChatClientMock>()
                .AddSingleton<IChatClient>(provider => provider.GetRequiredService<ChatClientMock>())
                .AddSingleton<IDispatcher, DispatcherMock>()
                .AddSingleton<IUserSession, UserSessionMock>()
                .AddTransient<ChatViewModel>();
        }
    }
}
