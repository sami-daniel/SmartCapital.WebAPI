using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.DTO.AddRequests;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.Filters;

namespace SmartCapital.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EntityFrameworkCoreTransactionControllerFilter]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Retorna todos os Perfis existentes
        /// </summary>
        /// <returns>Todos os Perfis exitentes</returns>
        /// <response code="200">Retorna todos os Perfis existentes</response>
        [HttpGet]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _profileService.GetAllProfilesAsync();

            return Ok(profiles.Select(p => p.ToProfileResponse()));
        }

        /// <summary>
        /// Retorna o Perfil de acordo com o nome
        /// </summary>
        /// <param name="profileName">O nome do Perfil</param>
        /// <returns>O Perfil que é identificado por aquele nome</returns>
        /// <response code="200">Retorna o Perfil de acordo com o nome especificado</response>
        /// <response code="404">Não foi possivel encontrar um Perfil com o nome especificado</response>
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
        /// Cria um novo perfil
        /// </summary>
        /// <param name="profile">O Perfil a ser criado</param>
        /// <returns>O Perfil recentemente criado</returns>
        /// <response code="201">O Perfil foi criado</response>
        /// <response code="400">Houve algum erro de validação no Perfil</response>
        [HttpPost]
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
        /// Deleta o Perfil de acordo com ID
        /// </summary>
        /// <param name="ID">O ID do perfil a ser deletado</param>
        /// <returns></returns>
        /// <response code="204">O Perfil foi deletado corretamente</response>
        /// <response code="404">Não foi possivel encontrar um Perfil com o ID especificado</response>
        [HttpDelete("{ID:int}")]
        public async Task<IActionResult> DeleteProfile([FromRoute] int ID)
        {
            var profileToRemove = await _profileService.GetProfileByIDAsync(ID);

            if (profileToRemove == null)
                return NotFound();

            await _profileService.RemoveProfileAsync(profileToRemove);

            return NoContent();
        }
    }
}
