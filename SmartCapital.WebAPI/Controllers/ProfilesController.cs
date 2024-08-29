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
    }
}
