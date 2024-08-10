using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Interfaces
{
    /// <summary>
    /// Define contratos para um repositório especializado na entidade <see cref="Profile"/>.
    /// </summary>
    public interface IProfileRepository : IRepository<Profile>
    {
    }
}
