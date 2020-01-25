using Chatterbox.Contracts.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Chatterbox.Server
{
    /// <summary>
    /// Initializes the services of the server application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Registers all services, which are needed to run the server.
        /// </summary>
        /// <param name="services">This instance of <see cref="IServiceCollection"/> will be used for the registration.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ConcurrentQueue<ChatMessage>>().AddSignalR();
        }

        /// <summary>
        /// Configures all services, which are used by the server.
        /// </summary>
        /// <param name="builder">This <see cref="IApplicationBuilder"/> is used for the configuration of the services.</param>
        public void Configure(IApplicationBuilder builder)
        {
            builder.UseRouting().UseEndpoints(endpoints => endpoints.MapHub<ChatHub>("/hubs/chat"));
        }
    }
}
