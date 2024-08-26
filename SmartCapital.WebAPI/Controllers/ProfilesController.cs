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
    /// Controlador responsável por gerenciar operações relacionadas a perfis.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        /// <returns>Uma lista de objetos <see cref="ProfileResponse"/> representando todos os perfis existentes no sistema.</returns>
        /// <response code="200">Perfis encontrados com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProfileResponse>), 200)]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _profileService.GetAllProfilesAsync();

            return Ok(profiles.Select(p => p.ToProfileResponse()));
        }

        /// <summary>
        /// Obtém o perfil correspondente ao nome fornecido.
        /// </summary>
        /// <param name="profileName">Nome do perfil a ser recuperado.</param>
        /// <returns>Um objeto <see cref="ProfileResponse"/> representando o perfil encontrado.</returns>
        /// <response code="200">Perfil encontrado com sucesso.</response>
        /// <response code="404">Perfil com o nome fornecido não encontrado.</response>
        [HttpGet("{profileName}")]
        [ProducesResponseType(typeof(ProfileResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> GetProfileByName([FromRoute] string profileName)
        {
            var profiles = await _profileService.GetFilteredProfilesAsync(p => p.ProfileName == profileName);

            if (profiles.Any())
            {
                return Ok(profiles.First().ToProfileResponse());
            }

            return NotFound(new ErrorResponse
            {
                ErrorType = "ProfileNotFound",
                Message = "O perfil com o nome fornecido não foi encontrado."
            });
        }

        /// <summary>
        /// Cria um novo perfil no sistema.
        /// </summary>
        /// <param name="profile">Dados do perfil a ser criado.</param>
        /// <returns>O perfil recém-criado.</returns>
        /// <response code="201">Perfil criado com sucesso.</response>
        /// <response code="400">Erro ao criar o perfil, devido a problemas de validação ou duplicidade.</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> AddProfile([FromBody] ProfileAddRequest profile)
        {
            try
            {
                await _profileService.AddProfileAsync(profile.ToProfile());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ValidationError",
                    Message = $"Erro de validação: {ex.Message}"
                });
            }
            catch (ExistingProfileException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ProfileCreationError",
                    Message = $"Erro ao criar o perfil: {ex.Message}"
                });
            }

            return CreatedAtRoute("", new { profile.ProfileName });
        }

        /// <summary>
        /// Exclui o perfil correspondente ao nome fornecido.
        /// </summary>
        /// <param name="profileName">Nome do perfil a ser excluído.</param>
        /// <returns>Status indicando o resultado da operação.</returns>
        /// <response code="204">Perfil excluído com sucesso.</response>
        /// <response code="404">Perfil com o nome fornecido não encontrado.</response>
        [HttpDelete("{profileName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> DeleteProfile([FromRoute] string profileName)
        {
            var profileToRemove = await _profileService.GetProfileByNameAsync(profileName);

            if (profileToRemove == null)
                return NotFound(new ErrorResponse
                {
                    ErrorType = "ProfileNotFound",
                    Message = "O perfil com o nome fornecido não foi encontrado."
                });

            await _profileService.RemoveProfileAsync(profileToRemove);

            return NoContent();
        }

        /// <summary>
        /// Atualiza um perfil existente com base no nome fornecido.
        /// </summary>
        /// <param name="profileName">Nome do perfil a ser atualizado.</param>
        /// <param name="profile">Dados atualizados do perfil.</param>
        /// <returns>Status indicando o resultado da operação.</returns>
        /// <response code="204">Perfil atualizado com sucesso.</response>
        /// <response code="400">Erro na atualização do perfil, devido a problemas de validação ou duplicidade.</response>
        /// <response code="404">Perfil com o nome fornecido não encontrado.</response>
        [HttpPut("{profileName}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        public async Task<IActionResult> UpdateProfile([FromRoute] string profileName, [FromBody] ProfileUpdateRequest profile)
        {
            Profile? updatedProfile;

            try
            {
                updatedProfile = await _profileService.UpdateProfileAsync(profileName, profile.ToProfile());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ValidationError",
                    Message = $"Erro de validação: {ex.Message}"
                });
            }
            catch (ExistingProfileException ex)
            {
                return BadRequest(new ErrorResponse
                {
                    ErrorType = "ProfileUpdateError",
                    Message = $"Erro ao atualizar o perfil: {ex.Message}"
                });
            }

            if (updatedProfile == null)
                return NotFound(new ErrorResponse
                {
                    ErrorType = "ProfileNotFound",
                    Message = "O perfil com o nome fornecido não foi encontrado."
                });

            return NoContent();
        }
    }
}
