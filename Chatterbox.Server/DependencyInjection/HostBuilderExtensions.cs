using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace Chatterbox.Server.DependencyInjection
{
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
        internal static IHostBuilder ConfigureServer(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection(LoggingSectionName));
                    LogManager.Configuration = new NLogLoggingConfiguration(context.Configuration.GetSection(NLogSectionName));
                });
        }
    }
}
