// none

using System.Linq.Expressions;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    /// <summary>
    /// Define os contratos para serviços relacionados a usuários.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adiciona um novo usuário ao sistema.
        /// </summary>
        /// <param name="userToAdd">O usuário a ser adicionado.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task AddUserAsync(User userToAdd);

        /// <summary>
        /// Atualiza um usuário existente no sistema.
        /// </summary>
        /// <param name="userName">O nome do usuário a ser modificado.</param>
        /// <param name="updatedUser">O objeto <see cref="User"/> contendo as informações atualizadas do usuário.</param>
        /// <returns>
        /// Uma tarefa que representa a operação assíncrona. O resultado da tarefa contém o objeto <see cref="User"/> atualizado se a atualização for bem-sucedida;
        /// caso contrário, retorna <c>null</c> se o usuário não for encontrado ou se a atualização falhar.
        /// </returns>
        public Task<User?> UpdateUserAsync(string userName, User updatedUser);

        /// <summary>
        /// Remove um usuário existente do sistema.
        /// </summary>
        /// <param name="userToRemove">O usuário a ser removido.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task RemoveUserAsync(User userToRemove);

        /// <summary>
        /// Obtém todos os usuários do sistema.
        /// </summary>
        /// <param name="filter">Uma expressão que define o critério de filtragem dos usuários.</param>
        /// <param name="includeProperties">Uma lista separada por vírgulas de propriedades de navegação a serem incluídas na consulta.</param>
        /// <param name="orderBy">Uma função que define a ordenação dos usuários.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de todos os usuários.</returns>
        public Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null, string includeProperties = "");

        /// <summary>
        /// Obtém o usuário de acordo com o nome.
        /// </summary>
        /// <param name="userName">O nome do usuário a ser obtido.</param>
        /// <returns>O usuário correspondente ao nome fornecido.</returns>
        /// <remarks>Se não for encontrado nenhum usuário, é retornado <c>null</c>.</remarks>
        public Task<User?> GetUserByNameAsync(string userName);
    }
}
