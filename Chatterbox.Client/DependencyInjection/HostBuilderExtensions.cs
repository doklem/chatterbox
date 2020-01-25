using AutoMapper;
using Chatterbox.Client.Cross.Abstractions;
using Chatterbox.Client.Cross.Implementations;
using Chatterbox.Client.DataAccess.Abstractions;
using Chatterbox.Client.DataAccess.Implementations;
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

namespace Chatterbox.Client.DependencyInjection
{
    /// <summary>
    /// This class provides extension methods for the <see cref="IHostBuilder"/>. In particular it serves as an encapsulation for the dependency injection
    /// registration process. It's the only place within the client, which is allowed to reference the actual implementations behind the interfaces.
    /// </summary>
    internal static class HostBuilderExtensions
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
        /// Configures all the services of the application on the given <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="hostBuilder">This <see cref="IHostBuilder"/> will obtain the various service registrations of the application.</param>
        /// <returns>Returns the given <paramref name="hostBuilder"/> again.</returns>
        internal static IHostBuilder ConfigureClient(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .UseNLog()
                .ConfigureLogging((context, builder) => builder.ConfigureLogging(context.Configuration))
                .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                .ConfigureServices((context, services) => services.ConfigureServices(context.HostingEnvironment, context.Configuration));
        }

        /// <summary>
        /// Configures the application logging.
        /// </summary>
        /// <param name="builder">This instance of a <see cref="ILoggingBuilder"/> will obtain the configuration.</param>
        /// <param name="configuration">This instance of a <see cref="IConfiguration"/> will be used for configuring the application logging.</param>
        /// <returns>Returns the given <paramref name="builder"/> again.</returns>
        private static ILoggingBuilder ConfigureLogging(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.AddConfiguration(configuration.GetSection(LoggingSectionName));
            LogManager.Configuration = new NLogLoggingConfiguration(configuration.GetSection(NLogSectionName));
            return builder;
        }

        /// <summary>
        /// Configures all the services of the application on the given <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">This <see cref="IServiceCollection"/> will obtain the various service registrations of the application</param>
        /// <param name="environment">Depending on this <see cref="IHostEnvironment"/> the SignalR standard logging will be configured accordingly.</param>
        /// <param name="configuration">This <see cref="IConfiguration"/> will be used as the source for the <see cref="AppSettings"/>.</param>
        /// <returns>Returns the given <paramref name="services"/> again.</returns>
        private static IServiceCollection ConfigureServices(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            return services
                    .Configure<AppSettings>(configuration)
                    .AddAutoMapper(typeof(HubConnectionStateProfile))
                    .AddTransient<IAsyncCommand, AsyncCommand>()
                    .AddSingleton(provider =>
                    {
                        var hubConnectionBuilder = new HubConnectionBuilder()
                            .ConfigureLogging(builder => builder.AddConfiguration(configuration.GetSection(LoggingSectionName)));
                        if (environment.IsDevelopment())
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
                    .AddSingleton<MainViewModel>()
                    .AddSingleton<MainWindow>()
                    // The following types need to be scoped. Like this they will dispose correctly during view changes.
                    .AddScoped<LoginViewModel>()
                    .AddScoped<LoginControl>()
                    .AddScoped<ChatViewModel>()
                    .AddScoped<ChatControl>();
        }
    }
}
