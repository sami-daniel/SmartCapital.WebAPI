using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.DTO.Login
{
    /// <summary>
    /// Represents the user login request, containing the credentials needed for authentication.
    /// </summary>
    public class UserLoginRequest
    {
        /// <summary>
        /// The username used for authentication. Must be provided and cannot be empty.
        /// </summary>
        [Required(ErrorMessage = "The Username cannot be empty.")]
        [StringLength(255, ErrorMessage = "The Username cannot exceed {0} characters.")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// The user's password used for authentication. Must be provided and cannot be empty.
        /// </summary>
        /// <example>yourpassword123</example>
        [Required(ErrorMessage = "The User Password cannot be empty.")]
        [StringLength(255, ErrorMessage = "The User Password cannot exceed {0} characters.")]
        public string UserPassword { get; set; } = null!;
    }
}
