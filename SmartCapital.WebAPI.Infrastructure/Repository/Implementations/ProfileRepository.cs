using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Implementations;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Implementations
{
    /// <summary>
    /// Implementa um repositório específico para a entidade <see cref="Profile"/>.
    /// </summary>
    public class ProfileRepository : Repository<Profile>, IProfileRepository
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ProfileRepository"/> com o contexto de banco de dados fornecido.
        /// </summary>
        /// <param name="applicationDbContext">O contexto de banco de dados usado para acessar a fonte de dados.</param>
        public ProfileRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
