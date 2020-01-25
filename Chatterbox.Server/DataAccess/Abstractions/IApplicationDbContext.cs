using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Chatterbox.Server.DataAccess.Abstractions
{
    /// <summary>
    /// The implementation of this interface encapsulates the database access.
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// Provides access to database related information and operations for this context.
        /// </summary>
        DatabaseFacade Database { get; }

        /// <summary>
        /// Gets the <see cref="DbSet{TEntity}"/>, which can be used to query message entities.
        /// </summary>
        DbSet<MessageEntity> Messages { get; }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <param name="cancellationToken">This optional <see cref="CancellationToken"/> could be used to abort the save process.</param>
        /// <returns>Returns the row count of the changes.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
