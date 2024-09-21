// none

using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Helpers;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI.Application.Implementations;

/// <summary>
/// Provides the implementation of profile-related services, including CRUD operations and filtering.
/// </summary>
public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileService"/> class with the provided unit of work.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used to manage repository operations and transactions.</param>
    public ProfileService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Adds a new profile to the system.
    /// </summary>
    /// <param name="profileToAdd">The profile to be added.</param>
    /// <param name="userName">The username of the user adding the profile.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the profile to be added is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the profile name is invalid.</exception>
    /// <exception cref="ExistingProfileException">Thrown when a profile with the same name already exists.</exception>
    public async Task AddProfileAsync(Profile profileToAdd, string userName)
    {
        ProfileValidationHelper.ValidateProfile(profileToAdd);

        profileToAdd.ProfileName = profileToAdd.ProfileName.Trim();

        var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName, includeProperties: "Profiles");

        var user = users.FirstOrDefault() ?? throw new ArgumentException("The user adding the profile was not found.");
        if (user.Profiles.Any(p => p.ProfileName == profileToAdd.ProfileName))
        {
            // FIXME: This check should also be implemented in the database
            // for greater consistency and integrity. The only thing protecting the user
            // from adding a profile with the same name is the application, which is not sufficient
            // and inconsistent.

            throw new ExistingProfileException($"A profile with the name {profileToAdd.ProfileName} already exists.");
        }

        using (var transaction = await _unitOfWork.StartTransactionAsync())
        {
            try
            {
                user.Profiles.Add(profileToAdd);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new ExistingProfileException($"A profile with the name {profileToAdd.ProfileName} already exists.");
            }
        }
    }

    /// <summary>
    /// Gets all profiles from the system.
    /// </summary>
    /// <param name="filter">An expression that defines the filtering criteria for the profiles.</param>
    /// <param name="orderBy">A function that defines the ordering of the profiles.</param>
    /// <param name="includeProperties">A comma-separated list of navigation properties to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The result is a collection of all profiles.</returns>
    public async Task<IEnumerable<Profile>> GetAllProfilesAsync(Expression<Func<Profile, bool>>? filter = null,
                                                                Func<IQueryable<Profile>, IOrderedQueryable<Profile>>? orderBy = null,
                                                                string includeProperties = "")
    {
        return await _unitOfWork.ProfileRepository.GetAsync(filter, orderBy, includeProperties);
    }

    /// <summary>
    /// Removes an existing profile from the system.
    /// </summary>
    /// <param name="profileToRemove">The profile to be removed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the profile to be removed is null.</exception>
    public async Task RemoveProfileAsync(Profile profileToRemove)
    {
        ArgumentNullException.ThrowIfNull(profileToRemove, "The profile to remove cannot be null.");

        _unitOfWork.ProfileRepository.Delete(profileToRemove);
        await _unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Updates an existing profile in the system.
    /// </summary>
    /// <param name="profileName">The name of the profile to be updated.</param>
    /// <param name="updatedProfile">The updated profile object.</param>
    /// <returns>A task that represents the asynchronous operation. The result is the updated profile object, or null if the profile is not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the profile name or the updated profile is null or invalid.</exception>
    /// <exception cref="ExistingProfileException">Thrown when a profile with the same name already exists.</exception>
    public async Task<Profile?> UpdateProfileAsync(string profileName, Profile updatedProfile)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(profileName, nameof(profileName));

        ProfileValidationHelper.ValidateProfile(updatedProfile);

        var profiles = await _unitOfWork.ProfileRepository.GetAsync(p => p.ProfileName == profileName);

        if (!profiles.Any())
        {
            return null;
        }

        var profile = profiles.First();

        if (profile.ProfileName.Length > 255)
        {
            throw new ArgumentException("The profile name cannot exceed 255 characters.");
        }

        if (!Regex.Match(profile.ProfileName, "^[a-zA-Z0-9 ]*$").Success)
        {
            throw new ArgumentException("The profile name can only contain letters, numbers, and spaces.");
        }

        if (profile.ProfileOpeningBalance != null)
        {
            if (profile.ProfileOpeningBalance > 999_999_999.99m)
            {
                throw new ArgumentException("The profile opening balance cannot be greater than 999,999,999.99.");
            }
        }

        updatedProfile.ProfileName = updatedProfile.ProfileName.Trim();

        profile.ProfileName = updatedProfile.ProfileName;
        profile.ProfileOpeningBalance = updatedProfile.ProfileOpeningBalance;

        using (var transaction = await _unitOfWork.StartTransactionAsync())
        {
            try
            {
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new ExistingProfileException($"A profile with the name {profile.ProfileName} already exists.");
            }
        }

        return profile;
    }
}
