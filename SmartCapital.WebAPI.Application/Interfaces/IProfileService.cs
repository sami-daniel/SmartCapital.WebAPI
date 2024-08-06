using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    public interface IProfileService
    {
        public Task AddProfileAsync(Profile profileToAdd);
        public Task RemoveProfileAsync(Profile profileToRemove);
    }
}
