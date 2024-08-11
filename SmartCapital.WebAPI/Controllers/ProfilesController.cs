using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.DTO.AddRequests;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.DTO.Responses;

namespace SmartCapital.WebAPI.Controllers
{
    /// <summary>
    /// Controlador para gerenciar operações relacionadas a perfis.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ProfilesController"/> com o serviço de perfis fornecido.
        /// </summary>
        /// <param name="profileService">O serviço para gerenciar operações de perfil.</param>
        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Retorna todos os perfis existentes.
        /// </summary>
        /// <returns>Todos os perfis existentes no sistema.</returns>
        /// <response code="200">Retorna uma lista de todos os perfis existentes.</response>
        [HttpGet]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _profileService.GetAllProfilesAsync();

            return Ok(profiles.Select(p => p.ToProfileResponse()));
        }

        /// <summary>
        /// Retorna o perfil de acordo com o nome fornecido.
        /// </summary>
        /// <param name="profileName">O nome do perfil a ser retornado.</param>
        /// <returns>O perfil que corresponde ao nome especificado.</returns>
        /// <response code="200">Retorna o perfil com o nome especificado.</response>
        /// <response code="404">Não foi possível encontrar um perfil com o nome especificado.</response>
        [HttpGet("{profileName}")]
        public async Task<IActionResult> GetProfileByName([FromRoute] string profileName)
        {
            var profiles = await _profileService.GetFilteredProfilesAsync(p => p.ProfileName == profileName);

            if (profiles.Any())
            {
                return Ok(profiles.First().ToProfileResponse());
            }

            return NotFound();
        }

        /// <summary>
        /// Cria um novo perfil.
        /// </summary>
        /// <param name="profile">Os dados do perfil a ser criado.</param>
        /// <returns>O perfil recentemente criado.</returns>
        /// <response code="201">O perfil foi criado com sucesso.</response>
        /// <response code="400">Houve um erro de validação nos dados do perfil.</response>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddProfile([FromBody] ProfileAddRequest profile)
        {
            try
            {
                await _profileService.AddProfileAsync(profile.ToProfile());
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Erro de validação de dados: {ex.Message}");
            }
            catch (ExistingProfileException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtRoute("", new { profile.ProfileName });
        }

        /// <summary>
        /// Deleta um perfil com o ID especificado.
        /// </summary>
        /// <param name="ID">O ID do perfil a ser deletado.</param>
        /// <returns>Um status de resposta indicando o resultado da operação.</returns>
        /// <response code="204">O perfil foi deletado com sucesso.</response>
        /// <response code="404">Não foi possível encontrar um perfil com o ID especificado.</response>
        [HttpDelete("{ID:int}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteProfile([FromRoute] uint ID)
        {
            var profileToRemove = await _profileService.GetProfileByIDAsync(ID);

            if (profileToRemove == null)
                return NotFound();

            await _profileService.RemoveProfileAsync(profileToRemove);

            return NoContent();
        }
    }
}
