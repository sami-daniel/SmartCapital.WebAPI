using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.DTO.UpdateRequests;
using SmartCapital.WebAPI.Models;

namespace SmartCapital.WebAPI.Controllers;

/// <summary>
/// Controller responsible for managing operations related to profiles.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    /// <summary>
    /// Initializes a new instance of <see cref="ProfilesController"/> with the provided profile service.
    /// </summary>
    /// <param name="profileService">Service to manage profile operations.</param>
    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Gets a list of all existing profiles.
    /// </summary>
    /// <returns>A list of <see cref="ProfileResponse"/> objects representing all user profiles.</returns>
    /// <response code="200">Profiles successfully found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfileResponse>), 200)]
    public async Task<IActionResult> GetProfiles()
    {
        var user = HttpContext.Items["User"] as string;

        if (string.IsNullOrEmpty(user))
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "User not found in the request context."
            });
        }

        var filteredProfiles = await _profileService.GetAllProfilesAsync(p => p.UsersUser.UserName == user);

        return Ok(filteredProfiles.Select(p => p.ToProfileResponse()));
    }

    /// <summary>
    /// Gets a profile by the specified name.
    /// </summary>
    /// <param name="profileName">The name of the user profile to be obtained.</param>
    /// <returns>The profile corresponding to the specified name.</returns>
    /// <response code="200">Profile successfully found.</response>
    /// <response code="404">Profile with the specified name was not found.</response>
    [HttpGet("{profileName}")]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> GetProfileByName([FromRoute] string profileName)
    {
        var user = HttpContext.Items["User"] as string;

        if (string.IsNullOrEmpty(user))
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "User not found in the request context."
            });
        }

        var filteredProfiles = await _profileService.GetAllProfilesAsync(p => p.UsersUser.UserName == user && p.ProfileName == profileName);

        var profile = filteredProfiles.FirstOrDefault();
        if (profile == null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorType = "ProfileFindError",
                Message = "The profile with the specified name was not found."
            });
        }

        return Ok(profile.ToProfileResponse());
    }

    /// <summary>
    /// Adds a new profile.
    /// </summary>
    /// <param name="profileAddRequest">Object containing the profile information to be added.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    /// <response code="201">Profile successfully created.</response>
    /// <response code="400">Error in the profile addition request, the request body is empty.</response>
    /// <response code="422">Error in the profile addition request, there are validation or duplication errors.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 422)]
    public async Task<IActionResult> AddProfile([FromBody] ProfileAddRequest profileAddRequest)
    {
        var name = HttpContext.Items["User"] as string;

        if (profileAddRequest == null)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "EmptyProfileAddRequest",
                Message = "The profile addition request cannot be null."
            });
        }

        if (string.IsNullOrEmpty(name))
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "User not found in the request context."
            });
        }

        try
        {
            await _profileService.AddProfileAsync(profileAddRequest.ToProfile(), name);
        }
        catch (ExistingProfileException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ProfileCreationError",
                Message = $"Error creating the Profile: {e.Message}"
            });
        }
        catch (ArgumentException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ValidationError",
                Message = $"Validation error: {e.Message}"
            });
        }

        return CreatedAtRoute("", new { profileAddRequest.ProfileName });
    }

    /// <summary>
    /// Updates an existing profile based on the specified profile name.
    /// </summary>
    /// <param name="profileName">The name of the profile to be updated.</param>
    /// <param name="profileUpdateRequest">Object containing the updated profile information.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    /// <response code="204">Profile successfully updated.</response>
    /// <response code="400">Error in the profile update request.</response>
    /// <response code="404">No profile found based on the name.</response>
    /// <response code="422">Error in the profile addition request, there are validation or duplication errors.</response>
    [HttpPut("{profileName}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 422)]
    public async Task<IActionResult> UpdateProfile([FromRoute] string profileName, [FromBody] ProfileUpdateRequest profileUpdateRequest)
    {
        if (profileUpdateRequest == null)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "EmptyProfileAddRequest",
                Message = "The profile addition request cannot be null."
            });
        }

        Profile? updatedProfile;

        try
        {
            updatedProfile = await _profileService.UpdateProfileAsync(profileName, profileUpdateRequest.ToProfile());
        }
        catch (ExistingProfileException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ProfileCreationError",
                Message = $"Error creating the Profile: {e.Message}"
            });
        }
        catch (ArgumentException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ValidationError",
                Message = $"Validation error: {e.Message}"
            });
        }

        if (updatedProfile == null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorType = "ProfileUpdateError",
                Message = "The profile with the specified name was not found."
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Removes an existing profile based on the specified profile name.
    /// </summary>
    /// <param name="profileName">The name of the profile to be removed.</param>
    /// <response code="404">The profile with the specified name was not found.</response>
    /// <response code="204">The profile was successfully removed.</response>
    [HttpDelete("{profileName}")]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteProfile([FromRoute] string profileName)
    {
        var name = HttpContext.Items["User"] as string;

        var profiles = await _profileService.GetAllProfilesAsync(p => p.UsersUser.UserName == name && p.ProfileName == profileName);
        var profile = profiles.FirstOrDefault();

        if (profile == null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorType = "ProfileNotFound",
                Message = "The profile with the provided name was not found."
            });
        }

        await _profileService.RemoveProfileAsync(profile);

        return NoContent();
    }
}
