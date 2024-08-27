using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace SmartCapital.WebAPI.Application.Implementations
{
    /// <summary>
    /// Fornece a implementa��o dos servi�os relacionados a usu�rios, incluindo opera��es CRUD e filtragem.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Inicializa uma nova inst�ncia da classe <see cref="UserService"/> com a unidade de trabalho fornecida.
        /// </summary>
        /// <param name="unitOfWork">A unidade de trabalho usada para gerenciar opera��es de reposit�rio e transa��es.</param>
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adiciona um novo usu�rio ao sistema.
        /// </summary>
        /// <param name="userToAdd">O usu�rio a ser adicionado.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona.</returns>
        /// <exception cref="ArgumentNullException">Lan�ada quando o usu�rio a ser adicionado � nulo.</exception>
        /// <exception cref="ArgumentException">Lan�ada quando o nome de usu�rio ou senha � inv�lido.</exception>
        /// <exception cref="ExistingUserException">Lan�ada quando um usu�rio com o mesmo nome j� existe.</exception>
        public async Task AddUserAsync(User userToAdd)
        {
            ArgumentNullException.ThrowIfNull(userToAdd, nameof(userToAdd));

            ArgumentException.ThrowIfNullOrEmpty(userToAdd.UserName, nameof(userToAdd.UserName));
            ArgumentException.ThrowIfNullOrEmpty(userToAdd.UserPassword, nameof(userToAdd.UserPassword));

            if (userToAdd.UserName.Length > 255 || userToAdd.UserName.Length <= 0)
                throw new ArgumentException("O tamanho do nome de usu�rio n�o pode exceder 255 caracteres e n�o pode ser vazio.");

            if (!Regex.Match(userToAdd.UserName, "^[a-zA-Z0-9 ]*$").Success)
            {
                throw new ArgumentException("O nome de usu�rio pode conter apenas letras, n�meros e espa�os.");
            }

            userToAdd.UserName = userToAdd.UserName.Trim();
            userToAdd.UserCreationDate = DateTime.UtcNow;

            using (var transaction = await _unitOfWork.StartTransactionAsync())
            {
                try
                {
                    await _unitOfWork.UserRepository.InsertAsync(userToAdd);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();
                    throw new ExistingUserException($"Um usu�rio com o nome {userToAdd.UserName} j� existe.");
                }
            }
        }

        /// <summary>
        /// Obt�m todos os usu�rios do sistema.
        /// </summary>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � uma cole��o de todos os usu�rios.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.UserRepository.GetAsync();
        }

        /// <summary>
        /// Obt�m usu�rios que correspondem ao filtro fornecido.
        /// </summary>
        /// <param name="filter">Uma express�o que define o crit�rio de filtragem dos usu�rios.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � uma cole��o de usu�rios que atendem ao crit�rio de filtro.</returns>
        public Task<IEnumerable<User>> GetFilteredUsersAsync(Expression<Func<User, bool>> filter)
        {
            return _unitOfWork.UserRepository.GetAsync(filter: filter);
        }

        /// <summary>
        /// Obt�m um usu�rio pelo nome.
        /// </summary>
        /// <param name="userName">O nome do usu�rio a ser obtido.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � o usu�rio com o nome especificado, ou nulo se nenhum usu�rio for encontrado.</returns>
        /// <exception cref="ArgumentException">Lan�ada quando o nome do usu�rio � nulo ou vazio.</exception>
        public async Task<User?> GetUserByNameAsync(string userName)
        {
            ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));

            var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

            return users.FirstOrDefault();
        }

        /// <summary>
        /// Remove um usu�rio existente do sistema.
        /// </summary>
        /// <param name="userToRemove">O usu�rio a ser removido.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona.</returns>
        /// <exception cref="ArgumentNullException">Lan�ada quando o usu�rio a ser removido � nulo.</exception>
        public async Task RemoveUserAsync(User userToRemove)
        {
            ArgumentNullException.ThrowIfNull(userToRemove, "O usu�rio a ser removido n�o pode ser nulo.");

            _unitOfWork.UserRepository.Delete(userToRemove);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Atualiza um usu�rio existente no sistema.
        /// </summary>
        /// <param name="userName">O nome do usu�rio a ser atualizado.</param>
        /// <param name="updatedUser">O objeto usu�rio atualizado.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � o objeto usu�rio atualizado, ou nulo se o usu�rio n�o for encontrado.</returns>
        /// <exception cref="ArgumentException">Lan�ada quando o nome do usu�rio ou o usu�rio atualizado � nulo ou inv�lido.</exception>
        /// <exception cref="ExistingUserException">Lan�ada quando um usu�rio com o mesmo nome j� existe.</exception>
        public async Task<User?> UpdateUserAsync(string userName, User updatedUser)
        {
            ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));
            ArgumentNullException.ThrowIfNull(updatedUser, nameof(updatedUser));

            var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

            if (!users.Any())
            {
                return null;
            }

            var user = users.First();

            if (updatedUser.UserName.Length > 255)
                throw new ArgumentException("O tamanho do nome de usu�rio n�o pode exceder 255 caracteres.");

            if (!Regex.Match(updatedUser.UserName, "^[a-zA-Z0-9 ]*$").Success)
            {
                throw new ArgumentException("O nome de usu�rio pode conter apenas letras, n�meros e espa�os.");
            }

            updatedUser.UserName = updatedUser.UserName.Trim();

            user.UserName = updatedUser.UserName;
            user.UserPassword = updatedUser.UserPassword;

            using (var transaction = await _unitOfWork.StartTransactionAsync())
            {
                try
                {
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();
                    throw new ExistingUserException($"Um usu�rio com o nome {user.UserName} j� existe.");
                }
            }

            return user;
        }
    }
}
