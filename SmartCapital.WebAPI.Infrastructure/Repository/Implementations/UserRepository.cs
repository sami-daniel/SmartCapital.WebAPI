using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Implementations;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Implementations
{
    /// <summary>
    /// Implementa um repositório específico para a entidade <see cref="User"/>.
    /// </summary>
    public class UserRepository : Repository<User> ,IUserRepository
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="UserRepository"/> com o contexto de banco de dados fornecido.
        /// </summary>
        /// <param name="applicationDbContext">O contexto de banco de dados usado para acessar a fonte de dados.</param>
        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
