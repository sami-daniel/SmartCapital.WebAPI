// none

using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    /// <summary>
    /// Defines the contracts for user-related services.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Gets the user according to the username and password.
        /// </summary>
        /// <param name="userName">The username of the user to be obtained.</param>
        /// <param name="pwd">The password of the user to be obtained.</param>
        /// <returns>The user corresponding to the provided username and password.</returns>
        /// <remarks>If no user is found, <c>null</c> is returned.</remarks>
        public Task<User?> GetUserAsync(string userName, string pwd);
    }
}
