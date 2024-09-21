// none

using Microsoft.EntityFrameworkCore.Storage;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces
{
    /// <summary>
    /// Defines contracts for a unit of work that manages repositories and database operations.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for the entity <see cref="Profile"/>.
        /// </summary>
        /// <value>
        /// An instance of the repository for the entity <see cref="Profile"/>.
        /// </value>
        public IProfileRepository ProfileRepository { get; }

        /// <summary>
        /// Gets the repository for the entity <see cref="User"/>.
        /// </summary>
        /// <value>
        /// An instance of the repository for the entity <see cref="User"/>.
        /// </value>
        public IUserRepository UserRepository { get; }

        /// <summary>
        /// Saves all changes made in the current database context.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The result is the number of changes made to the database.
        /// </returns>
        public Task<int> CompleteAsync();

        /// <summary>
        /// Starts a new transaction in the current database context.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The result is the transaction of the database context.
        /// </returns>
        public Task<IDbContextTransaction> StartTransactionAsync();
    }
}
