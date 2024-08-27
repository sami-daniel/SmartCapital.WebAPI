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
        /// Remove um usuário existente do sistema.
        /// </summary>
        /// <param name="userToRemove">O usuário a ser removido.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task RemoveUserAsync(User userToRemove);

        /// <summary>
        /// Obtém o usuário de acordo com o nome.
        /// </summary>
        /// <param name="userName">O nome do usuário a ser obtido.</param>
        /// <returns>O usuário correspondente ao nome fornecido.</returns>
        /// <remarks>Se não for encontrado nenhum usuário, é retornado <c>null</c>.</remarks>
        public Task<User?> GetProfileByNameAsync(string userName);

    }
}
