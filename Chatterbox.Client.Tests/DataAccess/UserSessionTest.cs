using Chatterbox.Client.DataAccess.Abstractions;
using Chatterbox.Client.DataAccess.Implementations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.Tests.DataAccess
{
    /// <summary>
    /// This is the unit test class for the <see cref="UserSession"/>.
    /// </summary>
    public class UserSessionTest : TestBase
    {
        /// <summary>
        /// Tests the <see cref="UserSession.LoginAsync(string)"/>-method on an empty session.
        /// </summary>
        [Test]
        public async Task LoginAsyncNewSession()
        {
            using var provider = CreateServiceProvider();
            var userName = "test user";
            using var userSession = provider.GetRequiredService<IUserSession>();
            Assert.IsNull(userSession.UserName);
            Assert.IsFalse(userSession.IsAuthenticated);
            Assert.IsTrue(await userSession.LoginAsync(userName));
            Assert.AreEqual(userName, userSession.UserName);
            Assert.IsTrue(userSession.IsAuthenticated);
        }

        /// <summary>
        /// Tests the <see cref="UserSession.LoginAsync(string)"/>-method on a session, which already has a user.
        /// </summary>
        [Test]
        public async Task LoginAsyncExistingSession()
        {
            using var provider = CreateServiceProvider();
            var userName = "Alice";
            using var userSession = provider.GetRequiredService<IUserSession>();
            await userSession.LoginAsync(userName);
            Assert.IsFalse(await userSession.LoginAsync("Bob"));
            Assert.AreEqual(userName, userSession.UserName);
            Assert.IsTrue(userSession.IsAuthenticated);
        }

        /// <summary>
        /// Tests the <see cref="UserSession.LoginAsync(string)"/>-method with illegal user names.
        /// </summary>
        /// <param name="emptyUserName">The <see cref="string"/> should be a <code>null</code> or empty value.</param>
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void LoginAsyncEmptyUserName(string emptyUserName)
        {
            using var provider = CreateServiceProvider();
            using var userSession = provider.GetRequiredService<IUserSession>();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await userSession.LoginAsync(emptyUserName));
            Assert.IsNull(userSession.UserName);
        }

        /// <summary>
        /// Tests the <see cref="UserSession.LogoutAsync"/>-method.
        /// </summary>
        [Test]
        public async Task LogoutAsync()
        {
            using var provider = CreateServiceProvider();
            using var userSession = provider.GetRequiredService<IUserSession>();
            await userSession.LoginAsync("test user");
            await userSession.LogoutAsync();
            Assert.IsFalse(userSession.IsAuthenticated);
            Assert.IsNull(userSession.UserName);
        }

        /// <summary>
        /// Tests the <see cref="UserSession.IsAuthenticatedChangedAsync"/>-event during a login attempt.
        /// </summary>
        [Test]
        public async Task IsAuthenticatedChangedAsyncLogin()
        {
            using var provider = CreateServiceProvider();
            using var userSession = provider.GetRequiredService<IUserSession>();
            bool raised = false;
            userSession.IsAuthenticatedChangedAsync += () =>
            {
                raised = true;
                return Task.CompletedTask;
            };
            await userSession.LoginAsync("test user");
            Assert.IsTrue(raised);
        }

        /// <summary>
        /// Tests the <see cref="UserSession.IsAuthenticatedChangedAsync"/>-event during a logout attempt.
        /// </summary>
        [Test]
        public async Task IsAuthenticatedChangedAsyncLogout()
        {
            using var provider = CreateServiceProvider();
            using var userSession = provider.GetRequiredService<IUserSession>();
            await userSession.LoginAsync("test user");
            bool raised = false;
            userSession.IsAuthenticatedChangedAsync += () =>
            {
                raised = true;
                return Task.CompletedTask;
            };
            await userSession.LogoutAsync();
            Assert.IsTrue(raised);
        }

        /// <inheritdoc/>
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserSession, UserSession>();
        }
    }
}