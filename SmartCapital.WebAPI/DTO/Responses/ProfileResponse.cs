using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.Responses;

/// <summary>
/// Represents the response that contains the details of a profile.
/// </summary>
public class ProfileResponse
{
    /// <summary>
    /// The creation date of the profile.
    /// </summary>
    public DateTime ProfileCreationDate { get; set; }

    /// <summary>
    /// The name of the profile.
    /// </summary>
    /// <remarks>
    /// This field should never be null.
    /// </remarks>
    public string ProfileName { get; set; } = null!;

    /// <summary>
    /// The initial balance of the profile.
    /// </summary>
    /// <remarks>
    /// This field is optional and can be null.
    /// </remarks>
    public decimal? ProfileOpeningBalance { get; set; }

    /// <summary>
    /// The user associated with the profile.
    /// </summary>
    public UserResponse User { get; set; } = null!;
}

/// <summary>
/// Contains extension methods for the <see cref="Profile"/> class.
/// </summary>
public static class ProfileExtensions
{
    /// <summary>
    /// Converts an instance of <see cref="Profile"/> to an instance of <see cref="ProfileResponse"/>.
    /// </summary>
    /// <param name="profile">The instance of <see cref="Profile"/> to be converted.</param>
    /// <returns>A new instance of <see cref="ProfileResponse"/> with the profile data.</returns>
    public static ProfileResponse ToProfileResponse(this Profile profile)
    {
        return new ProfileResponse
        {
            ProfileCreationDate = profile.ProfileCreationDate,
            ProfileOpeningBalance = profile.ProfileOpeningBalance,
            ProfileName = profile.ProfileName,
            User = profile.UsersUser.ToUserResponse()
        };
    }
}
