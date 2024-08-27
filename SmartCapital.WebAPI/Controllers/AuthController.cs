using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Login;
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

        /// <summary>
        /// Inicializa uma nova instância de <see cref="AuthController"/> com o serviço de usuários fornecido.
        /// </summary>
        /// <param name="userService">Serviço para gerenciar operações de usuários.</param>
        /// <param name="configuration">Set de configurações do projeto</param>
        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// Autentica um usuário e gera um token JWT se as credenciais forem válidas.
        /// </summary>
        /// <param name="userLoginRequest">O objeto contendo as credenciais de login do usuário.</param>
        /// <returns>Retorna um resultado de ação contendo o nome do usuário e o token JWT se a autenticação for bem-sucedida. Caso contrário, retorna um erro se o usuário não for encontrado.</returns>
        /// <response code="200">Retorna o nome do usuário, a Role e o token JWT.</response>
        /// <response code="404">Retorna um erro se o usuário com o nome especificado não for encontrado.</response>
        /// <response code="400">Retorna um erro se a solicitação estiver malformada.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginRequest userLoginRequest)
        {
            var user = await _userService.GetProfileByNameAsync(userLoginRequest.UserName!);

            if (user == null)
                return NotFound(new ErrorResponse
                {
                    ErrorType = "UserFindError",
                    Message = "O Usuário com o nome especificado não foi encontrado."
                });

            var token = GenerateToken(userLoginRequest);
            user.UserPassword = "";

            return Ok(new UserLoginResponse
            {
                User = user.UserName,
                Role = User.Claims.First(u => u.ValueType == ClaimTypes.Role).Value,
                Token = token
            });
        }

        /// <summary>
        /// Gera um token JWT para o usuário fornecido.
        /// </summary>
        /// <param name="user">O usuário para o qual o token JWT será gerado.</param>
        /// <returns>O token JWT gerado.</returns>
        /// <exception cref="InvalidOperationException">Lançado quando a configuração do segredo JWT está ausente.</exception>
        [NonAction]
        public string GenerateToken(UserLoginRequest user)
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
