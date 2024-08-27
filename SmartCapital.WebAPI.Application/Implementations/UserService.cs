using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;
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

        public async Task AddUserAsync(User userToAdd)
        {
            ArgumentNullException.ThrowIfNull(userToAdd, nameof(userToAdd));

            ArgumentException.ThrowIfNullOrEmpty(userToAdd.UserName, nameof(userToAdd.UserName));

            ArgumentException.ThrowIfNullOrEmpty(userToAdd.UserPassword, nameof(userToAdd.UserPassword));

            if (userToAdd.UserName.Length > 255 || userToAdd.UserName.Length <= 0)
                throw new ArgumentException("O tamanho do Nome do Usuário não pode exceder 255 caracteres e não pode ser vazio.");

            if (userToAdd.UserPassword.Length > 255 || userToAdd.UserPassword.Length <= 0)
                throw new ArgumentException("O tamanho da Senha do Usuário não pode exceder 255 caracteres e não pode ser vazio.");

            userToAdd.UserName = userToAdd.UserName.Trim();
            
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
                    throw new ExistingUserException($"Um Usuário com o nome {userToAdd.UserName} já existe.");
                }
            }
        }

        public async Task<User?> GetProfileByNameAsync(string userName)
        {
            ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));

            var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

            return users.FirstOrDefault();
        }

        public async Task RemoveUserAsync(User userToRemove)
        {
            ArgumentNullException.ThrowIfNull(userToRemove, nameof(userToRemove));

            _unitOfWork.UserRepository.Delete(userToRemove);

            await _unitOfWork.CompleteAsync();
        }
    }
}
