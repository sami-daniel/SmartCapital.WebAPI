// none

using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Interfaces
{
    /// <summary>
    /// Define os contratos para serviços relacionados a usuários.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Obtém o usuário de acordo com o nome e senha.
        /// </summary>      
        /// <param name="userName">O nome do usuário a ser obtido.</param>
        /// <param name="pwd">A senha do usuário a ser obtido.</param>
        /// <returns>O usuário correspondente ao nome fornecido e a senha.</returns>
        /// <remarks>Se não for encontrado nenhum usuário, é retornado <c>null</c>.</remarks>
        public Task<User?> GetUserAsync(string userName, string pwd);

    }
}
