using SmartCapital.WebAPI.Domain.Domain;
using System.Linq.Expressions;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    public interface IProfileService
    {
        public Task AddProfileAsync(Profile profileAddRequest);
        public Task RemoveProfileAsync(Profile profileToRemove);
        public Task<Profile?> GetProfileByIDAsync(int profileID);
        public Task<IEnumerable<Profile>> GetAllProfilesAsync();
        public Task<IEnumerable<Profile>> GetFilteredProfilesAsync(Expression<Func<Profile, bool>> filter);
    }
}
