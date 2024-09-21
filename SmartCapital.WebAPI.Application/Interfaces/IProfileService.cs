// none

using System.Linq.Expressions;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces;

/// <summary>
/// Defines the contracts for profile-related services.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Adds a new profile to the system.
    /// </summary>
    /// <param name="profileToAdd">The profile to be added.</param>
    /// <param name="userName">The username of the user adding the profile.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddProfileAsync(Profile profileToAdd, string userName);

    /// <summary>
    /// Updates an existing profile in the system.
    /// </summary>
    /// <param name="profileName">The name of the profile to be updated.</param>
    /// <param name="updatedProfile">The <see cref="Profile"/> object containing the updated profile information.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the updated <see cref="Profile"/> object if the update is successful;
    /// otherwise, returns <c>null</c> if the profile is not found or if the update fails.
    /// </returns>
    public Task<Profile?> UpdateProfileAsync(string profileName, Profile updatedProfile);

    /// <summary>
    /// Removes an existing profile from the system.
    /// </summary>
    /// <param name="profileToRemove">The profile to be removed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RemoveProfileAsync(Profile profileToRemove);

    /// <summary>
    /// Gets all profiles from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The result is a collection of all profiles.</returns>
    public Task<IEnumerable<Profile>> GetAllProfilesAsync(Expression<Func<Profile, bool>>? filter = null,
                                                          Func<IQueryable<Profile>, IOrderedQueryable<Profile>>? orderBy = null,
                                                          string includeProperties = "");
}
