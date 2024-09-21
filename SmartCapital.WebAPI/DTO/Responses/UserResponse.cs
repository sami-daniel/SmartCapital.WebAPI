using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.Responses
{
    /// <summary>
    /// Represents the response that contains the details of a user.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// The creation date of the user.
        /// </summary>
        public DateTime UserCreationDate { get; set; }

        /// <summary>
        /// The collection of profiles associated with the user.
        /// </summary>
        public virtual ICollection<ProfileResponse> Profiles { get; set; } = new List<ProfileResponse>();
    }

    /// <summary>
    /// Contains extension methods for the <see cref="User"/> class.
    /// </summary>
    public static class UserResponseExtensions
    {
        /// <summary>
        /// Converts an instance of <see cref="User"/> to an instance of <see cref="UserResponse"/>.
        /// </summary>
        /// <param name="user">The instance of <see cref="User"/> to be converted.</param>
        /// <returns>A new instance of <see cref="UserResponse"/> with the user data.</returns>
        public static UserResponse ToUserResponse(this User user)
        {
            return new UserResponse
            {
                UserName = user.UserName,
                UserCreationDate = user.UserCreationDate,
                Profiles = user.Profiles.Select(p => p.ToProfileResponse()).ToList()
            };
        }
    }
}
