using Chatterbox.Server.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Chatterbox.Server.DataAccess.Implementations
{
    /// <inheritdoc/>
    internal class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        /// <inheritdoc/>
        public DbSet<MessageEntity> Messages { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
