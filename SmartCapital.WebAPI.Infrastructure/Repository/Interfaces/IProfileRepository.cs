// none

using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Interfaces
{
    /// <summary>
    /// Defines contracts for a repository specialized in the entity <see cref="Profile"/>.
    /// </summary>
    public interface IProfileRepository : IRepository<Profile>
    {
    }
}
