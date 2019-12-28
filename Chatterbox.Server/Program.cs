using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using System.Threading.Tasks;

namespace Chatterbox.Server
{
    /// <summary>
    /// The server's main class.
    /// </summary>
    public class Program
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
        /// The main entry point for the server.
        /// </summary>
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .UseNLog()
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection(LoggingSectionName));
                    LogManager.Configuration = new NLogLoggingConfiguration(context.Configuration.GetSection(NLogSectionName));
                })
                .Build()
                .RunAsync();
        }
    }
}
