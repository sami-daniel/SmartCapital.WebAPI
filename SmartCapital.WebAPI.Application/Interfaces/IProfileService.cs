// none

using System.Linq.Expressions;
using SmartCapital.WebAPI.Domain.Domain;

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
        /// <param name="profileToAdd">O perfil a ser adicionado.</param>
        /// <param name="userName">O nome do usuário que está adicionando o perfil.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task AddProfileAsync(Profile profileToAdd, string userName);

        /// <summary>
        /// Atualiza um perfil existente no sistema.
        /// </summary>
        /// <param name="profileName">O nome do perfil a ser modificado.</param>
        /// <param name="profile">O objeto <see cref="Profile"/> contendo as informações atualizadas do perfil.</param>
        /// <returns>
        /// Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o objeto <see cref="Profile"/> atualizado se a atualização for bem-sucedida;
        /// caso contrário, retorna <c>null</c> se o perfil não for encontrado ou se a atualização falhar.
        /// </returns>
        public Task<Profile?> UpdateProfileAsync(string profileName, Profile updatedProfile);

        /// <summary>
        /// Remove um perfil existente do sistema.
        /// </summary>
        /// <param name="profileToRemove">O perfil a ser removido.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task RemoveProfileAsync(Profile profileToRemove);

        /// <summary>
        /// Obtém todos os perfis do sistema.
        /// </summary>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de todos os perfis.</returns>
        public Task<IEnumerable<Profile>> GetAllProfilesAsync(Expression<Func<Profile, bool>>? filter = null,
                                                                    Func<IQueryable<Profile>, IOrderedQueryable<Profile>>? orderBy = null,
                                                                    string includeProperties = "");

        /// <summary>
        /// Obtém o perfil de acordo com o nome.
        /// </summary>
        /// <param name="profileName">O nome do perfil a ser obtido.</param>
        /// <returns>O perfil correspondente ao nome fornecido.</returns>
        /// <remarks>Se não for encontrado nenhum perfil, é retornado <c>null</c>.</remarks>
        public Task<Profile?> GetProfileByNameAsync(string profileName);
    }
}
