using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    public interface IProfileService
    {
        public Task AddProfileAsync(Profile profileAddRequest);
        public Task RemoveProfileAsync(Profile profileToRemove);
        public Task<Profile?> GetProfileByIDAsync(int profileID);
    }
}
