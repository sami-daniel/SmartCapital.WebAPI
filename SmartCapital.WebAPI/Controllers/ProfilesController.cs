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
    }
}
