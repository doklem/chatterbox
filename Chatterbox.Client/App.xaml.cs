using Chatterbox.Client.DataAccess;
using Chatterbox.Client.Helpers;
using Chatterbox.Client.Options;
using Chatterbox.Client.ViewModels;
using Chatterbox.Client.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using System.Windows;
using System.Windows.Threading;

namespace Chatterbox.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Gets the name of the configuration section for logging.
        /// </summary>
        private const string LoggingSectionName = "Logging";

        /// <summary>
        /// Gets the name of the configuration section for NLog.
        /// </summary>
        private const string NLogSectionName = LoggingSectionName + ":NLog";

        /// <summary>
        /// Gets the <see cref="IHost"/>, which will be used to created the various services of the client through dependency injection.
        /// </summary>
        private readonly IHost host;

        /// <summary>
        /// Creates a new instance of <see cref="App"/>.
        /// </summary>
        public App()
        {
            Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            host = Host.CreateDefaultBuilder()
                .UseNLog()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection(LoggingSectionName));
                    LogManager.Configuration = new NLogLoggingConfiguration(context.Configuration.GetSection(NLogSectionName));
                })
                .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                .ConfigureServices((context, services) => services
                    .Configure<AppSettings>(context.Configuration)
                    .AddTransient<IAsyncCommand, AsyncCommand>()
                    .AddSingleton(provider =>
                    {
                        var hubConnectionBuilder = new HubConnectionBuilder()
                            .ConfigureLogging(builder => builder.AddConfiguration(context.Configuration.GetSection(LoggingSectionName)));
                        if (context.HostingEnvironment.IsDevelopment())
                        {
                            hubConnectionBuilder = hubConnectionBuilder.ConfigureLogging(builder => builder.AddDebug());
                        }
                        return hubConnectionBuilder;
                    })
                    .AddSingleton<ChatClient>()
                    .AddSingleton<IHostedService>(provider => provider.GetRequiredService<ChatClient>())
                    .AddSingleton<IChatClient>(provider => provider.GetRequiredService<ChatClient>())
                    .AddSingleton<IDispatcher, DispatcherAdapter>()
                    .AddSingleton<IUserSession, UserSession>()
                    .AddScoped<LoginViewModel>()
                    .AddScoped<LoginControl>()
                    .AddScoped<ChatViewModel>()
                    .AddScoped<ChatControl>()
                    .AddSingleton<MainViewModel>()
                    .AddSingleton<MainWindow>())
                .Build();
        }

        /// <summary>
        /// Since the main way of WPF "Application.Startup" will not work with the dependency injection, we need to create the <see cref="MainWindow"/> here.
        /// </summary>
        protected override async void OnStartup(StartupEventArgs args)
        {
            await host.StartAsync();
            host.Services.GetRequiredService<MainWindow>().Show();
            base.OnStartup(args);
        }

        /// <summary>
        /// Stops and disposes the <see cref="host"/> during application shutdown.
        /// </summary>
        protected override void OnExit(ExitEventArgs args)
        {
            using (host)
            {
                host.StopAsync().FireAndForgetSafeAsync();
            }
            base.OnExit(args);
        }

        /// <summary>
        /// Handels <see cref="System.Exception"/>, which where not catched by showing the with the <see cref="FallbackExceptionHandler"/>.
        /// If the <see cref="System.Exception"/> was created after the initialization of the logging, it will be logged as well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            LogManager.GetCurrentClassLogger()?.Error(args.Exception);
            FallbackExceptionHandler.ShowError(args.Exception);
            args.Handled = true;
        }
    }
}
