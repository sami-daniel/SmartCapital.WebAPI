// none

using System.Linq.Expressions;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    /// <summary>
    /// Define os contratos para servi�os relacionados a usu�rios.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adiciona um novo usu�rio ao sistema.
        /// </summary>
        /// <param name="userToAdd">O usu�rio a ser adicionado.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona.</returns>
        public Task AddUserAsync(User userToAdd);

        /// <summary>
        /// Atualiza um usu�rio existente no sistema.
        /// </summary>
        /// <param name="userName">O nome do usu�rio a ser modificado.</param>
        /// <param name="updatedUser">O objeto <see cref="User"/> contendo as informa��es atualizadas do usu�rio.</param>
        /// <returns>
        /// Uma tarefa que representa a opera��o ass�ncrona. O resultado da tarefa cont�m o objeto <see cref="User"/> atualizado se a atualiza��o for bem-sucedida;
        /// caso contr�rio, retorna <c>null</c> se o usu�rio n�o for encontrado ou se a atualiza��o falhar.
        /// </returns>
        public Task<User?> UpdateUserAsync(string userName, User updatedUser);

        /// <summary>
        /// Remove um usu�rio existente do sistema.
        /// </summary>
        /// <param name="userToRemove">O usu�rio a ser removido.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona.</returns>
        public Task RemoveUserAsync(User userToRemove);

        /// <summary>
        /// Obt�m todos os usu�rios do sistema.
        /// </summary>
        /// <param name="filter">Uma express�o que define o crit�rio de filtragem dos usu�rios.</param>
        /// <param name="includeProperties">Uma lista separada por v�rgulas de propriedades de navega��o a serem inclu�das na consulta.</param>
        /// <param name="orderBy">Uma fun��o que define a ordena��o dos usu�rios.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � uma cole��o de todos os usu�rios.</returns>
        public Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null, string includeProperties = "");

        /// <summary>
        /// Obt�m o usu�rio de acordo com o nome.
        /// </summary>
        /// <param name="userName">O nome do usu�rio a ser obtido.</param>
        /// <returns>O usu�rio correspondente ao nome fornecido.</returns>
        /// <remarks>Se n�o for encontrado nenhum usu�rio, � retornado <c>null</c>.</remarks>
        public Task<User?> GetUserByNameAsync(string userName);
    }
}
