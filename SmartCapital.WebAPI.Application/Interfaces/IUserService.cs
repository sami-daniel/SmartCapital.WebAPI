using SmartCapital.WebAPI.Domain.Domain;
using System.Linq.Expressions;

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
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � uma cole��o de todos os usu�rios.</returns>
        public Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Obt�m usu�rios que correspondem ao filtro fornecido.
        /// </summary>
        /// <param name="filter">Uma express�o que define o crit�rio de filtragem dos usu�rios.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � uma cole��o de usu�rios que atendem ao crit�rio de filtro.</returns>
        public Task<IEnumerable<User>> GetFilteredUsersAsync(Expression<Func<User, bool>> filter);

        /// <summary>
        /// Obt�m o usu�rio de acordo com o nome.
        /// </summary>
        /// <param name="userName">O nome do usu�rio a ser obtido.</param>
        /// <returns>O usu�rio correspondente ao nome fornecido.</returns>
        /// <remarks>Se n�o for encontrado nenhum usu�rio, � retornado <c>null</c>.</remarks>
        public Task<User?> GetUserByNameAsync(string userName);
    }
}
