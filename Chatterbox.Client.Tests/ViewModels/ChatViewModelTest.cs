using Chatterbox.Client.DataAccess;
using Chatterbox.Client.Helpers;
using Chatterbox.Client.Options;
using Chatterbox.Client.Tests.Mocks;
using Chatterbox.Client.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.ViewModels
{
    /// <summary>
    /// This is the unit test class for the <see cref="MainViewModel"/>.
    /// </summary>
    public class ChatViewModelTest
    {
        /// <summary>
        /// Gets the name of the sender, which will be used within the tests.
        /// </summary>
        private const string SenderName = "Alice";

        /// <summary>
        /// Gets or sets the <see cref="IServiceCollection"/>.
        /// </summary>
        private IServiceCollection services;

        /// <summary>
        /// Setup for the test class. It does the registrations on the <see cref="services"/>.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>() { { "Sender", SenderName } })
                .Build();
            services = new ServiceCollection().AddLogging()
                .AddOptions()
                .Configure<AppSettings>(config)
                .AddTransient<IAsyncCommand, AsyncCommand>()
                .AddSingleton<ChatClientMock>()
                .AddSingleton<IChatClient>(provider => provider.GetRequiredService<ChatClientMock>())
                .AddSingleton<DispatcherMock>()
                .AddSingleton<IDispatcher>(provider => provider.GetRequiredService<DispatcherMock>())
                .AddTransient<MainViewModel>();
        }

        /// <summary>
        /// Tests if changes on the <see cref="MainViewModel.NewMessage"/>-property will raise a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [Test]
        public async Task NewMessagePropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised = string.Equals(e.PropertyName, nameof(viewModel.NewMessage));
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
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event of the <see cref="MainViewModel.ConnectionState"/>.
        /// </summary>
        [Test]
        public async Task ConnectionStatePropertyChangedEvent()
        {
            var provider = CreateServiceProvider();
            try
            {
                var chatClientMock = provider.GetRequiredService<ChatClientMock>();
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var raised = false;
                viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) => raised = string.Equals(e.PropertyName, nameof(viewModel.ConnectionState));
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
        /// Tests if the <see cref="MainViewModel.SendCommand"/>-command will trigger a new message on the <see cref="IChatClient"/>.
        /// </summary>
        [Test]
        public async Task SendCommand()
        {
            var provider = CreateServiceProvider();
            try
            {
                var chatClientMock = provider.GetRequiredService<ChatClientMock>();
                await chatClientMock.SetStateAsync(ConnectionState.Connected).ConfigureAwait(false);
                var text = "test message";
                var viewModel = provider.GetRequiredService<MainViewModel>();
                viewModel.NewMessage = text;
                await viewModel.SendCommand.ExecuteAsync().ConfigureAwait(false);
                var message = chatClientMock.Messages.Last();
                Assert.AreEqual(text, message.Text);
                Assert.AreEqual(SenderName, message.Sender);
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
        /// Tests the <see cref="MainViewModel.SendCommand"/>'s <see cref="AsyncCommand.CanExecute"/>-method depending on
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
                var viewModel = provider.GetRequiredService<MainViewModel>();
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

        /// <summary>
        /// Creates a new <see cref="ServiceProvider"/> based upon the <see cref="services"/>.
        /// </summary>
        /// <returns>Returns a new <see cref="ServiceProvider"/> based upon the <see cref="services"/>.
        /// The caller remains responsible for it's disposal.</returns>
        private ServiceProvider CreateServiceProvider()
        {
            return services.BuildServiceProvider();
        }
    }
}
