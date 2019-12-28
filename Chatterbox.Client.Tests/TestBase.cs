using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Chatterbox.Client.Tests
{
    /// <summary>
    /// This class serves as the abstract base for all unit tests.
    /// </summary>
    public abstract class TestBase
    {
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
            services = new ServiceCollection().AddLogging();
            ConfigureServices(services);
        }

        /// <summary>
        /// Creates a new <see cref="ServiceProvider"/> based upon the <see cref="services"/>.
        /// </summary>
        /// <returns>Returns a new <see cref="ServiceProvider"/> based upon the <see cref="services"/>.
        /// The caller remains responsible for it's disposal.</returns>
        protected ServiceProvider CreateServiceProvider()
        {
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// A hook for subclasses, which will add registrations to the <see cref="services"/> during setup.
        /// </summary>
        /// <param name="services">This <see cref="IServiceCollection"/> will obtain the registrations.</param>
        protected virtual void ConfigureServices(IServiceCollection services) { }
    }
}
