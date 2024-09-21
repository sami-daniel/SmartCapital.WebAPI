using System.ComponentModel.DataAnnotations;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.AddRequests
{
    /// <summary>
    /// Represents a request to add a new profile.
    /// </summary>
    public class ProfileAddRequest
    {
        /// <summary>
        /// Profile name. Must be a maximum of 255 characters.
        /// </summary>
        /// <remarks>
        /// This field is required and cannot be null.
        /// </remarks>
        [Required]
        [StringLength(255, ErrorMessage = "The Profile Name length cannot exceed {0} characters")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "The Profile Name can only contain letters and numbers.")]
        public string ProfileName { get; set; } = null!;

        /// <summary>
        /// Initial balance of the profile. Must be within the allowed range of 0 to 999,999,999.99.
        /// </summary>
        /// <remarks>
        /// This field is optional and can be null.
        /// </remarks>
        [Range(0d, 999_999_999.99d, ErrorMessage = "The Profile Opening Balance cannot be greater than {1}.")]
        public decimal? ProfileOpeningBalance { get; set; }

        /// <summary>
        /// Converts the current instance of <see cref="ProfileAddRequest"/> to an instance of <see cref="Profile"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="Profile"/> with the profile data.</returns>
        public Profile ToProfile()
        {
            return new Profile
            {
                ProfileName = ProfileName,
                ProfileOpeningBalance = ProfileOpeningBalance
            };
        }
    }
}
