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
    /// Fornece a implementação dos serviços relacionados a usuários, incluindo operações CRUD e filtragem.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="UserService"/> com a unidade de trabalho fornecida.
        /// </summary>
        /// <param name="unitOfWork">A unidade de trabalho usada para gerenciar operações de repositório e transações.</param>
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adiciona um novo usuário ao sistema.
        /// </summary>
        /// <param name="userToAdd">O usuário a ser adicionado.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando o usuário a ser adicionado é nulo.</exception>
        /// <exception cref="ArgumentException">Lançada quando o nome de usuário ou senha é inválido.</exception>
        /// <exception cref="ExistingUserException">Lançada quando um usuário com o mesmo nome já existe.</exception>
        public async Task AddUserAsync(User userToAdd)
        {
            ArgumentNullException.ThrowIfNull(userToAdd, nameof(userToAdd));

            ArgumentException.ThrowIfNullOrEmpty(userToAdd.UserName, nameof(userToAdd.UserName));
            ArgumentException.ThrowIfNullOrEmpty(userToAdd.UserPassword, nameof(userToAdd.UserPassword));

            if (userToAdd.UserName.Length > 255 || userToAdd.UserName.Length <= 0)
                throw new ArgumentException("O tamanho do nome de usuário não pode exceder 255 caracteres e não pode ser vazio.");

            if (!Regex.Match(userToAdd.UserName, "^[a-zA-Z0-9 ]*$").Success)
            {
                throw new ArgumentException("O nome de usuário pode conter apenas letras, números e espaços.");
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
                    throw new ExistingUserException($"Um usuário com o nome {userToAdd.UserName} já existe.");
                }
            }
        }

        /// <summary>
        /// Obtém todos os usuários do sistema.
        /// </summary>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de todos os usuários.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _unitOfWork.UserRepository.GetAsync();
        }

        /// <summary>
        /// Obtém usuários que correspondem ao filtro fornecido.
        /// </summary>
        /// <param name="filter">Uma expressão que define o critério de filtragem dos usuários.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de usuários que atendem ao critério de filtro.</returns>
        public Task<IEnumerable<User>> GetFilteredUsersAsync(Expression<Func<User, bool>> filter)
        {
            return _unitOfWork.UserRepository.GetAsync(filter: filter);
        }

        /// <summary>
        /// Obtém um usuário pelo nome.
        /// </summary>
        /// <param name="userName">O nome do usuário a ser obtido.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é o usuário com o nome especificado, ou nulo se nenhum usuário for encontrado.</returns>
        /// <exception cref="ArgumentException">Lançada quando o nome do usuário é nulo ou vazio.</exception>
        public async Task<User?> GetUserByNameAsync(string userName)
        {
            ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));

            var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

            return users.FirstOrDefault();
        }

        /// <summary>
        /// Remove um usuário existente do sistema.
        /// </summary>
        /// <param name="userToRemove">O usuário a ser removido.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        /// <exception cref="ArgumentNullException">Lançada quando o usuário a ser removido é nulo.</exception>
        public async Task RemoveUserAsync(User userToRemove)
        {
            ArgumentNullException.ThrowIfNull(userToRemove, "O usuário a ser removido não pode ser nulo.");

            _unitOfWork.UserRepository.Delete(userToRemove);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Atualiza um usuário existente no sistema.
        /// </summary>
        /// <param name="userName">O nome do usuário a ser atualizado.</param>
        /// <param name="updatedUser">O objeto usuário atualizado.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é o objeto usuário atualizado, ou nulo se o usuário não for encontrado.</returns>
        /// <exception cref="ArgumentException">Lançada quando o nome do usuário ou o usuário atualizado é nulo ou inválido.</exception>
        /// <exception cref="ExistingUserException">Lançada quando um usuário com o mesmo nome já existe.</exception>
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
                throw new ArgumentException("O tamanho do nome de usuário não pode exceder 255 caracteres.");

            if (!Regex.Match(updatedUser.UserName, "^[a-zA-Z0-9 ]*$").Success)
            {
                throw new ArgumentException("O nome de usuário pode conter apenas letras, números e espaços.");
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
                    throw new ExistingUserException($"Um usuário com o nome {user.UserName} já existe.");
                }
            }

            return user;
        }
    }
}
