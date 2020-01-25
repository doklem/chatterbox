using AutoMapper;
using Chatterbox.Server.Business.Abstractions;
using Chatterbox.Server.Business.Implementations;
using Chatterbox.Server.DataAccess.Abstractions;
using Chatterbox.Server.DataAccess.Implementations;
using Chatterbox.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatterbox.Server.DependencyInjection
{
    /// <summary>
    /// Initializes the services of the server application.
    /// </summary>
    internal class Startup
    {
        /// <summary>
        /// Gets the application's <see cref="IConfiguration"/>.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Creates a new instance of <see cref="Startup"/>.
        /// </summary>
        /// <param name="configuration">This instance of <see cref="IConfiguration"/> will be used as the application's configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Registers all services, which are needed to run the server.
        /// </summary>
        /// <param name="services">This instance of <see cref="IServiceCollection"/> will be used for the registration.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutoMapper(typeof(ChatMessageProfile))
                .AddDbContext<IApplicationDbContext, ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(nameof(ApplicationDbContext))))
                .AddScoped<IMessageManager, MessageManager>()
                .AddSignalR();
        }

        /// <summary>
        /// Configures all services, which are used by the server.
        /// </summary>
        /// <param name="builder">This <see cref="IApplicationBuilder"/> is used for the configuration of the services.</param>
        public void Configure(IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<IApplicationDbContext>().Database.EnsureCreated();
            }
            builder.UseRouting().UseEndpoints(endpoints => endpoints.MapHub<ChatHub>("/hubs/chat"));
        }
    }
}
