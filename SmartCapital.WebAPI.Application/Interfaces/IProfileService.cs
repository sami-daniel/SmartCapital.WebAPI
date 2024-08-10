using SmartCapital.WebAPI.Domain.Domain;
using System.Linq.Expressions;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    /// <summary>
    /// Define os contratos para serviços relacionados a perfis.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Adiciona um novo perfil ao sistema.
        /// </summary>
        /// <param name="profileAddRequest">O perfil a ser adicionado.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task AddProfileAsync(Profile profileAddRequest);

        /// <summary>
        /// Remove um perfil existente do sistema.
        /// </summary>
        /// <param name="profileToRemove">O perfil a ser removido.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task RemoveProfileAsync(Profile profileToRemove);

        /// <summary>
        /// Obtém um perfil específico pelo seu identificador.
        /// </summary>
        /// <param name="profileID">O identificador do perfil a ser recuperado.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é o perfil com o identificador especificado ou <c>null</c> se não encontrado.</returns>
        public Task<Profile?> GetProfileByIDAsync(uint profileID);

        /// <summary>
        /// Obtém todos os perfis do sistema.
        /// </summary>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de todos os perfis.</returns>
        public Task<IEnumerable<Profile>> GetAllProfilesAsync();

        /// <summary>
        /// Obtém perfis que correspondem ao filtro fornecido.
        /// </summary>
        /// <param name="filter">Uma expressão que define o critério de filtragem dos perfis.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de perfis que atendem ao critério de filtro.</returns>
        public Task<IEnumerable<Profile>> GetFilteredProfilesAsync(Expression<Func<Profile, bool>> filter);
    }
}
