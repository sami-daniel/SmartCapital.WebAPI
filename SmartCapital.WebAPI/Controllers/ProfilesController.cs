using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.DTO.UpdateRequests;
using SmartCapital.WebAPI.Models;

namespace SmartCapital.WebAPI.Controllers;

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
    /// <returns>Uma lista de objetos <see cref="ProfileResponse"/> representado todos os perfis do usuário.</returns>
    /// <response code="200">Perfis encontrados com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfileResponse>), 200)]
    public async Task<IActionResult> GetProfiles()
    {
        var user = HttpContext.Items["User"] as string;

        if (string.IsNullOrEmpty(user))
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "Usuário não encontrado no contexto da solicitação."
            });
        }

        var filteredProfiles = await _profileService.GetAllProfilesAsync(p => p.UsersUser.UserName == user);

        return Ok(filteredProfiles.Select(p => p.ToProfileResponse()));
    }

    /// <summary>
    /// Obtém um perfil pelo nome especificado.
    /// </summary>
    /// <param name="profileName">Nome do perfil do usuário a ser obtido.</param>
    /// <returns>O perfil correspondente ao nome especificado.</returns>
    /// <response code="200">Perfil encontrado com sucesso.</response>
    /// <response code="404">Perfil com o nome especificado não foi encontrado.</response>
    [HttpGet("{profileName}")]
    [ProducesResponseType(typeof(ProfileResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> GetProfileByName([FromRoute] string profileName)
    {
        var user = HttpContext.Items["User"] as string;

        if (string.IsNullOrEmpty(user))
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "Usuário não encontrado no contexto da solicitação."
            });
        }

        var filteredProfiles = await _profileService.GetAllProfilesAsync(p => p.UsersUser.UserName == user && p.ProfileName == profileName);

        var profile = filteredProfiles.FirstOrDefault();
        if (profile == null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorType = "ProfileFindError",
                Message = "O perfil com o nome especificado não foi encontrado."
            });
        }

        return Ok(profile.ToProfileResponse());
    }

    /// <summary>
    /// Adiciona um novo perfil.
    /// </summary>
    /// <param name="profileAddRequest">Objeto contendo as informações do perfil a ser adicionado.</param>
    /// <returns>Um resultado indicando o sucesso ou falha da operação.</returns>
    /// <response code="201">Perfil criado com sucesso.</response>
    /// <response code="400">Erro na solicitação de adição de perfil, o corpo da requisição é vazio.</response>
    /// <response code="422">Erro na solicitação de adição de perfil, o tem erros de validação ou duplicidade.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 422)]
    public async Task<IActionResult> AddProfile([FromBody] ProfileAddRequest profileAddRequest)
    {
        var name = HttpContext.Items["User"] as string;

        if (profileAddRequest == null)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "EmptyProfileAddRequest",
                Message = "A solicitação de adição de perfil não pode ser nula."
            });
        }

        if (string.IsNullOrEmpty(name))
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "Usuário não encontrado no contexto da solicitação."
            });
        }

        try
        {
            await _profileService.AddProfileAsync(profileAddRequest.ToProfile(), name);
        }
        catch (ExistingProfileException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ProfileCreationError",
                Message = $"Erro ao criar o Perfil: {e.Message}"
            });
        }
        catch (ArgumentException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ValidationError",
                Message = $"Erro de validação: {e.Message}"
            });
        }

        return CreatedAtRoute("", new { profileAddRequest.ProfileName });
    }

    /// <summary>
    /// Atualiza um perfil existente com base no nome do perfil especificado.
    /// </summary>
    /// <param name="profileName">Nome do perfil a ser atualizado.</param>
    /// <param name="profileUpdateRequest">Objeto contendo as informações atualizadas do perfil.</param>
    /// <returns>Um resultado indicando o sucesso ou falha da operação.</returns>
    /// <response code="204">Perfil atualizado com sucesso.</response>
    /// <response code="400">Erro na solicitação de atualização de perfil.</response>
    /// <response code="404">Não foi encontrado nenhum perfil com base no nome.</response>
    /// <response code="422">Erro na solicitação de adição de perfil, o tem erros de validação ou duplicidade.</response>
    [HttpPut("{profileName}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 422)]
    public async Task<IActionResult> UpdateProfile([FromRoute] string profileName, [FromBody] ProfileUpdateRequest profileUpdateRequest)
    {
        if (profileUpdateRequest == null)
        {
            return BadRequest(new ErrorResponse
            {
                ErrorType = "EmptyProfileAddRequest",
                Message = "A solicitação de adição de perfil não pode ser nula."
            });
        }

        Profile? updatedProfile;

        try
        {
            updatedProfile = await _profileService.UpdateProfileAsync(profileName, profileUpdateRequest.ToProfile());
        }
        catch (ExistingProfileException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ProfileCreationError",
                Message = $"Erro ao criar o Perfil: {e.Message}"
            });
        }
        catch (ArgumentException e)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ValidationError",
                Message = $"Erro de validação: {e.Message}"
            });
        }

        if (updatedProfile == null)
        {
            return NotFound(new ErrorResponse
            {
                ErrorType = "ProfileUpdateError",
                Message = "O perfil com o nome especificado não foi encontrado."
            });
        }

        return NoContent();
    }
}
