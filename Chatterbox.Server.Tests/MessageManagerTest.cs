using AutoMapper;
using Chatterbox.Server.Business.Abstractions;
using Chatterbox.Server.Business.Implementations;
using Chatterbox.Server.DataAccess.Abstractions;
using Chatterbox.Server.DataAccess.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Chatterbox.Server.Tests
{
    /// <summary>
    /// This is the unit test class for the <see cref="MessageManager"/>.
    /// </summary>
    public class MessageManagerTest
    {
        /// <summary>
        /// Gets or sets the <see cref="IServiceCollection"/>.
        /// </summary>
        private IServiceCollection services;

        /// <summary>
        /// Tests if the <see cref="IMessageManager.CreateAsync(string, string, System.Threading.CancellationToken)"/>-method does use the
        /// given <see cref="string"/>s for the new <see cref="Contracts.Messages.ChatMessage"/>.
        /// </summary>
        [Test]
        public async Task CreateAsync()
        {
            using var provider = services.BuildServiceProvider();
            var manager = provider.GetRequiredService<IMessageManager>();
            var sender = "Test sender";
            var text = "Test text";
            var message = await manager.CreateAsync(sender, text).ConfigureAwait(false);
            Assert.IsNotNull(message);
            Assert.AreEqual(sender, message.Sender);
            Assert.AreEqual(text, message.Text);
        }

        /// <summary>
        /// Tests if the <see cref="IMessageManager.CreateAsync(string, string, System.Threading.CancellationToken)"/>-method does persist the
        /// new <see cref="Contracts.Messages.ChatMessage"/> as a <see cref="MessageEntity"/> in the database.
        /// </summary>
        [Test]
        public async Task CreateAsyncInDatabase()
        {
            using var provider = services.BuildServiceProvider();
            var dbContext = provider.GetRequiredService<IApplicationDbContext>();
            var sender = "Test sender";
            var text = "Test text";
            var message = await provider.GetRequiredService<IMessageManager>().CreateAsync(sender, text).ConfigureAwait(false);
            var entity = dbContext.Messages.Find(message.Id);
            Assert.IsNotNull(entity);
            Assert.AreEqual(sender, entity.Sender);
            Assert.AreEqual(text, entity.Text);
        }

        /// <summary>
        /// Tests if the <see cref="IMessageManager.CreateAsync(string, string, System.Threading.CancellationToken)"/>-method does rise a
        /// <see cref="ArgumentNullException"/>, when the given sender is <code>null</code>.
        /// </summary>
        [Test]
        public void CreateAsyncWithoutSender()
        {
            using var provider = services.BuildServiceProvider();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await provider
                .GetRequiredService<IMessageManager>()
                .CreateAsync(null, "Test text")
                .ConfigureAwait(false));
        }

        /// <summary>
        /// Tests if the <see cref="IMessageManager.CreateAsync(string, string, System.Threading.CancellationToken)"/>-method does rise a
        /// <see cref="ArgumentNullException"/>, when the given text is <code>null</code>.
        /// </summary>
        [Test]
        public void CreateAsyncWithoutText()
        {
            using var provider = services.BuildServiceProvider();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await provider
                .GetRequiredService<IMessageManager>()
                .CreateAsync("Test sender", null)
                .ConfigureAwait(false));
        }

        /// <summary>
        /// Tests if the <see cref="IMessageManager.GetAllAsync(System.Threading.CancellationToken)"/>-method does return all
        /// <see cref="Contracts.Messages.ChatMessage"/>s.
        /// </summary>
        [Test]
        public async Task GetAllAsync()
        {
            using var provider = services.BuildServiceProvider();
            var dbContext = provider.GetRequiredService<IApplicationDbContext>();
            dbContext.Messages.Add(new MessageEntity { Sender = "Test sender 1", Text = "Test text 1", Time = DateTime.UtcNow });
            dbContext.Messages.Add(new MessageEntity { Sender = "Test sender 2", Text = "Test text 2", Time = DateTime.UtcNow });
            await dbContext.SaveChangesAsync().ConfigureAwait(false);
            var expectedMessages = await dbContext.Messages.ToListAsync().ConfigureAwait(false);
            var actualMessages = await provider.GetRequiredService<IMessageManager>().GetAllAsync().ConfigureAwait(false);
            Assert.AreEqual(expectedMessages.Count, actualMessages.Count());
            foreach (var expectedMessage in expectedMessages)
            {
                var actualMessage = actualMessages.SingleOrDefault(message => message.Id == expectedMessage.Id);
                Assert.IsNotNull(actualMessage);
                Assert.AreEqual(expectedMessage.Sender, actualMessage.Sender);
                Assert.AreEqual(expectedMessage.Text, actualMessage.Text);
            }
        }

        /// <summary>
        /// Setup for the test class. It does the registrations on the <see cref="services"/>.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            services = new ServiceCollection()
                .AddLogging()
                .AddAutoMapper(typeof(ChatMessageProfile))
                .AddDbContext<IApplicationDbContext, ApplicationDbContext>(options => options.UseInMemoryDatabase("Chatterbox"))
                .AddScoped<IMessageManager, MessageManager>();
        }
    }
}