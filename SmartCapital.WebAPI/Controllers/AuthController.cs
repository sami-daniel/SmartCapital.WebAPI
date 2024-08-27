using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartCapital.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar operações relacionadas a autenticação.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            var user = await _userService.GetProfileByNameAsync(userLoginRequest.UserName!);

            if (user == null)
                return NotFound(new ErrorResponse
                {
                    ErrorType = "UserFindError",
                    Message = "O Usuário com o nome especificado não foi encontrado."
                });

            var token = GenerateToken(user.ToUserResponse());
            user.UserPassword = "";

            return Ok(new
            {
                User = user.UserName,
                Token = token
            });
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserAddRequest userAddRequest)
        {
            try
            {
                await _userService.AddUserAsync(userAddRequest.ToUser());
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ValidationError",
                    Message = $"Ocorreu um erro de validação de dados: {ex.Message}"
                });
            }
            catch (ExistingUserException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "UserCreationError",
                    Message = $"Ocorreu um erro na criação do usuário: {ex.Message}"
                });
            }

            return Created();
        }

        [NonAction]
        public string GenerateToken(UserResponse user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWTSettings:Secret"] ?? throw new InvalidOperationException("O token do JWT (secret) não está definido."));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "APIUser")
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
