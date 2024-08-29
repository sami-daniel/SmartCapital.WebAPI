using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.Models;

namespace SmartCapital.WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar operações relacionadas a perfis.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ProfilesController"/> com o serviço de perfis fornecido.
        /// </summary>
        /// <param name="profileService">Serviço para gerenciar operações de perfil.</param>
        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Obtém uma lista de todos os perfis existentes.
        /// </summary>
        /// <returns>Uma lista de objetos <see cref="ProfileResponse"/> representado todos os perfis existente no sistem</returns>
        /// <response code="200">Perfis encontrados com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProfileResponse>), 200)]
        public async Task<IActionResult> GetProfiles()
        {
            var user = HttpContext.Items["User"] as string;
            var role = HttpContext.Items["Role"] as string;

            if (role == "Admin")
            {
                var profiles = await _profileService.GetAllProfilesAsync();

                return Ok(profiles);
            }

            var filteredProfiles = await _profileService.GetAllProfilesAsync(p => p.UsersUser.UserName == user);

            return Ok(filteredProfiles.Select(p => p.ToProfileResponse()));
        }

        /// <summary>
        /// Obtém um perfil pelo nome especificado.
        /// </summary>
        /// <param name="profileName">Nome do perfil a ser obtido.</param>
        /// <returns>O perfil correspondente ao nome especificado.</returns>
        /// <response code="200">Perfil encontrado com sucesso.</response>
        /// <response code="404">Perfil com o nome especificado não foi encontrado.</response>
        [HttpGet("{profileName}")]
        [ProducesResponseType(typeof(ProfileResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetProfileByName(string profileName)
        {
            var user = HttpContext.Items["User"] as string;
            var role = HttpContext.Items["Role"] as string;

            if (role == "Admin")
            {
                var profile = await _profileService.GetProfileByNameAsync(profileName);

                if (profile == null)
                    return NotFound(new ErrorResponse
                    {
                        ErrorType = "ProfileFindError",
                        Message = "O perfil com o nome especificado não foi encontrado."
                    });

                return Ok(profile);
            }

            var filteredProfiles = await _profileService.GetAllProfilesAsync(p => p.UsersUser.UserName == user && p.ProfileName == profileName);

            if (!filteredProfiles.Any())
                return NotFound(new ErrorResponse
                {
                    ErrorType = "ProfileFindError",
                    Message = "O perfil com o nome especificado não foi encontrado."
                });

            return Ok(filteredProfiles.First());
        }

        public async Task<IActionResult> AddProfile([FromBody] ProfileAddRequest profileAddRequest)
        {
            var name = HttpContext.Items["User"] as string;

            if (profileAddRequest == null)
                return BadRequest(new ErrorResponse()
                {
                    ErrorType = "EmptyProfileAddRequest",
                    Message = "A solicitação de adição de perfil não pode ser nula."
                });

            try
            {
                await _profileService.AddProfileAsync(profileAddRequest.ToProfile());
            }
            catch (ExistingProfileException e)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ProfileCreationError",
                    Message = $"Erro ao criar o Perfil: {e.Message}"
                });
            }
            catch (ArgumentException e)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ValidationError",
                    Message = $"Erro de validação: {e.Message}"
                });
            }

            return CreatedAtAction("GetProfileByName", new { profileName = profileAddRequest.ProfileName });
        }
    }
}
