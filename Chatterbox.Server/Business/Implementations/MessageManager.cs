using AutoMapper;
using AutoMapper.QueryableExtensions;
using Chatterbox.Contracts.Messages;
using Chatterbox.Server.Business.Abstractions;
using Chatterbox.Server.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Chatterbox.Server.Business.Implementations
{
    /// <inheritdoc/>
    internal class MessageManager : IMessageManager
    {
        /// <summary>
        /// Gets the <see cref="IApplicationDbContext"/>, which will be used to persist the <see cref="ChatMessage"/>s.
        /// </summary>
        private readonly IApplicationDbContext dbContext;

        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used for logging the persisted messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets the <see cref="IMapper"/>, which will be used to convert <see cref="MessageEntity"/> into <see cref="ChatMessage"/>.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Creates a new instance of <see cref="MessageManager"/>.
        /// </summary>
        /// <param name="dbContext">This instance of <see cref="IApplicationDbContext"/> will become the manager's <paramref name="dbContext"/>.</param>
        /// <param name="logger">This instance of <see cref="ILogger"/> will become the manager's <see cref="logger"/>.</param>
        /// <param name="mapper">This instance of <see cref="IMapper"/> will become the manager's <paramref name="mapper"/>.</param>
        public MessageManager(IApplicationDbContext dbContext, ILogger<MessageManager> logger, IMapper mapper)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<ChatMessage> CreateAsync(string sender, string text, CancellationToken cancellationToken = default)
        {
            var entity = new MessageEntity
            {
                Sender = sender ?? throw new ArgumentNullException(nameof(sender)),
                Text = text ?? throw new ArgumentNullException(nameof(text)),
                Time = DateTime.UtcNow
            };
            logger.LogDebug("Saving a message from {sender} with the text: {text}.", sender, text);
            dbContext.Messages.Add(entity);
            await dbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
            return mapper.Map<ChatMessage>(entity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ChatMessage>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Fetching all messages.");
            return await dbContext.Messages
                .ProjectTo<ChatMessage>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}