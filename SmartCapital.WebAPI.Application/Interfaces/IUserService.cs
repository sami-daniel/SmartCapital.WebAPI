// none

using System.Linq.Expressions;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    /// <summary>
    /// Defines the contracts for user-related services.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <param name="userToAdd">The user to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task AddUserAsync(User userToAdd);

        /// <summary>
        /// Updates an existing user in the system.
        /// </summary>
        /// <param name="userName">The name of the user to be updated.</param>
        /// <param name="updatedUser">The <see cref="User"/> object containing the updated user information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the updated <see cref="User"/> object if the update is successful;
        /// otherwise, returns <c>null</c> if the user is not found or if the update fails.
        /// </returns>
        public Task<User?> UpdateUserAsync(string userName, User updatedUser);

        /// <summary>
        /// Removes an existing user from the system.
        /// </summary>
        /// <param name="userToRemove">The user to be removed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task RemoveUserAsync(User userToRemove);

        /// <summary>
        /// Gets all users from the system.
        /// </summary>
        /// <param name="filter">An expression that defines the filtering criteria for the users.</param>
        /// <param name="includeProperties">A comma-separated list of navigation properties to include in the query.</param>
        /// <param name="orderBy">A function that defines the ordering of the users.</param>
        /// <returns>A task that represents the asynchronous operation. The result is a collection of all users.</returns>
        public Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null, string includeProperties = "");

        /// <summary>
        /// Gets the user according to the name.
        /// </summary>
        /// <param name="userName">The name of the user to be obtained.</param>
        /// <returns>The user corresponding to the provided name.</returns>
        /// <remarks>If no user is found, <c>null</c> is returned.</remarks>
        public Task<User?> GetUserByNameAsync(string userName);
    }
}
