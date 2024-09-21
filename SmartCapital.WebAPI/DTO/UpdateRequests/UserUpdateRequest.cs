using System.ComponentModel.DataAnnotations;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.UpdateRequests
{
    /// <summary>
    /// Represents a request to update an existing user.
    /// </summary>
    public class UserUpdateRequest
    {
        /// <summary>
        /// The username. Must be a maximum of 255 characters.
        /// </summary>
        /// <remarks>
        /// This field is required and cannot be null.
        /// </remarks>
        [Required(ErrorMessage = "The Username cannot be empty.")]
        [StringLength(255, ErrorMessage = "The Username length cannot exceed {0} characters")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "The Username can only contain letters and numbers.")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// The password. This field is required and cannot be null.
        /// </summary>
        [Required(ErrorMessage = "The password cannot be empty.")]
        public string UserPassword { get; set; } = null!;

        /// <summary>
        /// Converts the current instance of <see cref="UserUpdateRequest"/> to an instance of <see cref="User"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="User"/> with the user data.</returns>
        public User ToUser()
        {
            return new User
            {
                UserName = UserName,
                UserPassword = UserPassword
            };
        }
    }
}
