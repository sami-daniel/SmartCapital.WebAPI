namespace SmartCapital.WebAPI.DTO.Login
{
    /// <summary>
    /// Represents the user authentication response, containing information about the authenticated user and the generated JWT token.
    /// </summary>
    public class UserLoginResponse
    {
        /// <summary>
        /// The name of the authenticated user.
        /// </summary>
        /// <example>user@example.com</example>
        public string User { get; set; } = null!;

        /// <summary>
        /// The role or function of the user in the system.
        /// </summary>
        /// <example>APIUser</example>
        public string Role { get; set; } = null!;

        /// <summary>
        /// The JWT token generated for the authenticated user.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string Token { get; set; } = null!;
    }
}
