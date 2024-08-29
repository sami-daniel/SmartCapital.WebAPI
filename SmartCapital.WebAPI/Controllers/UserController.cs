using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.DTO.UpdateRequests;
using SmartCapital.WebAPI.Models;

namespace SmartCapital.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar operações relacionadas a usuários.
    /// </summary>
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="UserController"/> com o serviço de usuários fornecido.
        /// </summary>
        /// <param name="userService">Serviço para gerenciar operações de usuário.</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtém uma lista de todos os usuários existentes.
        /// </summary>
        /// <returns>Uma lista de objetos <see cref="UserResponse"/> representando todos os usuários existentes no sistema.</returns>
        /// <response code="200">Usuários encontrados com sucesso.</response>
        /// <response code="403">Não autorizado o acesso ao recurso.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), 200)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users.Select(u => u.ToUserResponse()));
        }

        /// <summary>
        /// Obtém o usuário correspondente ao nome fornecido.
        /// </summary>
        /// <param name="userName">Nome do usuário a ser recuperado.</param>
        /// <returns>Um objeto <see cref="UserResponse"/> representando o usuário encontrado.</returns>
        /// <response code="200">Usuário encontrado com sucesso.</response>
        /// <response code="404">Usuário com o nome fornecido não encontrado.</response>
        /// <response code="403">Não autorizado o acesso ao recurso.</response>
        
        [HttpGet("{userName}")]
        [ProducesResponseType(typeof(UserResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUserByName([FromRoute] string userName)
        {
            var users = await _userService.GetFilteredUsersAsync(u => u.UserName == userName);
            var userNameFromToken = HttpContext.Items["User"] as string;

            var userFromToken = await _userService.GetUserByNameAsync(userNameFromToken!);


            if (users.Any())
            {
                var usrFirst = users.First();
                
                if (userFromToken!.UserName != usrFirst.UserName)
                {
                    return Forbid();
                }


                return Ok(usrFirst);
            }

            

            return NotFound(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "O usuário com o nome fornecido não foi encontrado."
            });
        }

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <param name="user">Dados do usuário a ser criado.</param>
        /// <returns>O usuário recém-criado.</returns>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Erro ao criar o usuário, devido a problemas de validação ou duplicidade.</response>
        
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> AddUser([FromBody] UserAddRequest user)
        {
            try
            {
                await _userService.AddUserAsync(user.ToUser());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ValidationError",
                    Message = $"Erro de validação: {ex.Message}"
                });
            }
            catch (ExistingUserException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "UserCreationError",
                    Message = $"Erro ao criar o usuário: {ex.Message}"
                });
            }

            return CreatedAtRoute("", new { user.UserName });
        }

        /// <summary>
        /// Exclui o usuário correspondente ao nome fornecido.
        /// </summary>
        /// <param name="userName">Nome do usuário a ser excluído.</param>
        /// <returns>Status indicando o resultado da operação.</returns>
        /// <response code="204">Usuário excluído com sucesso.</response>
        /// <response code="403">Não autorizado o acesso ao recurso.</response>
        /// <response code="404">Usuário com o nome fornecido não encontrado.</response>
        [HttpDelete("{userName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> DeleteUser([FromRoute] string userName)
        {
            var userToRemove = await _userService.GetUserByNameAsync(userName);
            
            var userNameFromToken = HttpContext.Items["User"] as string;

            if (userToRemove == null)
                return NotFound(new ErrorResponse
                {
                    ErrorType = "UserNotFound",
                    Message = "O usuário com o nome fornecido não foi encontrado."
                });

            if (userNameFromToken != userToRemove.UserName)
            {
                return Forbid();
            }

            await _userService.RemoveUserAsync(userToRemove);

            return NoContent();
        }

        /// <summary>
        /// Atualiza um usuário existente com base no nome fornecido.
        /// </summary>
        /// <param name="userName">Nome do usuário a ser atualizado.</param>
        /// <param name="user">Dados atualizados do usuário.</param>
        /// <returns>Status indicando o resultado da operação.</returns>
        /// <response code="204">Usuário atualizado com sucesso.</response>
        /// <response code="400">Erro na atualização do usuário, devido a problemas de validação ou duplicidade.</response>
        /// <response code="403">Não autorizado o acesso ao recurso.</response>
        /// <response code="404">Usuário com o nome fornecido não encontrado.</response>
        [HttpPut("{userName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> UpdateUser([FromRoute] string userName, [FromBody] UserUpdateRequest user)
        {
            User? updatedUser;
            
            var userNameFromToken = HttpContext.Items["User"] as string;

            try
            {
                if (userNameFromToken != userName)
                {
                    return Forbid();
                }

                updatedUser = await _userService.UpdateUserAsync(userName, user.ToUser());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ValidationError",
                    Message = $"Erro de validação: {ex.Message}"
                });
            }
            catch (ExistingUserException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "UserUpdateError",
                    Message = $"Erro ao atualizar o usuário: {ex.Message}"
                });
            }

            if (updatedUser == null)
                return NotFound(new ErrorResponse
                {
                    ErrorType = "UserNotFound",
                    Message = "O usuário com o nome fornecido não foi encontrado."
                });

            return NoContent();
        }
    }
}