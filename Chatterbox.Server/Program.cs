using Chatterbox.Server.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Chatterbox.Server.Tests")]
namespace Chatterbox.Server
{
    /// <summary>
    /// The server's main class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the server.
        /// </summary>
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServer()
                .Build()
                .RunAsync();
        }
    }
}
