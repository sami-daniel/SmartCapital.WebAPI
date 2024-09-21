using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.DTO.Login;
using SmartCapital.WebAPI.Models;

namespace SmartCapital.WebAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing authentication-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginService _userService;

        /// <summary>
        /// Initializes a new instance of <see cref="AuthController"/> with the provided user service.
        /// </summary>
        /// <param name="userService">Service to manage user operations.</param>
        /// <param name="configuration">Project configuration settings.</param>
        public AuthController(IConfiguration configuration, ILoginService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token if the credentials are valid.
        /// </summary>
        /// <param name="userLoginRequest">The object containing the user's login credentials.</param>
        /// <returns>Returns an action result containing the username and JWT token if authentication is successful. Otherwise, returns an error if the user is not found.</returns>
        /// <response code="200">Returns the username, role, and JWT token.</response>
        /// <response code="404">Returns an error if the user with the specified name is not found.</response>
        /// <response code="400">Returns an error if the request is malformed.</response>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(UserLoginResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginRequest userLoginRequest)
        {
            var user = await _userService.GetUserAsync(userLoginRequest.UserName, userLoginRequest.UserPassword);

            if (user == null)
                return NotFound(new ErrorResponse
                {
                    ErrorType = "UserFindError",
                    Message = "The user with the specified name and password was not found."
                });

            var token = GenerateToken(userLoginRequest);
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var role = jwtToken.Claims.First(claim => claim.Type == "role").Value;

            return Ok(new UserLoginResponse
            {
                User = user.UserName,
                Role = role,
                Token = token
            });
        }

        /// <summary>
        /// Generates a JWT token for the provided user.
        /// </summary>
        /// <param name="user">The user for whom the JWT token will be generated.</param>
        /// <returns>The generated JWT token.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the JWT secret configuration is missing.</exception>
        [NonAction]
        public string GenerateToken(UserLoginRequest user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWTSettings:Secret"] ?? throw new InvalidOperationException("The JWT secret is not defined."));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "APIUser")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
