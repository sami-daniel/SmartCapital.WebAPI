using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.DTO.AddRequests;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.Filters;

namespace SmartCapital.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EntityFrameworkCoreTransactionControllerFilter]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _profileService.GetAllProfilesAsync();

            return Ok(profiles.Select(p => p.ToProfileResponse()));
        }


        [HttpPost]
        public async Task<IActionResult> AddProfile([FromBody] ProfileAddRequest profile)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join('\n', ModelState.Values.SelectMany(e => e.Errors));
                return BadRequest(errors);
            }
            await _profileService.AddProfileAsync(profile.ToProfile());

            return Created();
        }

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
