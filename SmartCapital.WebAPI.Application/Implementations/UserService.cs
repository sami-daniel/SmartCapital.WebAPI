// none

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Helpers;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI.Application.Implementations;

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
    /// <exception cref="ExistingUserException">Lançada quando um usuário com o mesmo nome já existe.</exception>
    public async Task AddUserAsync(User userToAdd)
    {
        UserValidationHelper.ValidateUser(userToAdd);

        userToAdd.UserName = userToAdd.UserName.Trim();
        userToAdd.UserPassword = userToAdd.UserPassword.Trim();

        userToAdd.UserPassword = BCrypt.Net.BCrypt.HashPassword(userToAdd.UserPassword);

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
    /// <remarks>Essa API não pode ser utilizada para modificar ou deletar nenhuma instância de <see cref="User"/>. Utilize <see cref="GetUserByNameAsync(string)"/> para alterar ou deletar instância em vez disso.</remarks>
    public async Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null, string includeProperties = "")
    {
        var users = await _unitOfWork.UserRepository.GetAsync(filter, orderBy, includeProperties, nonTrackableEntities: true);

        foreach (var user in users)
        {
            user.UserPassword = string.Empty;
        }

        return users;
    }

    /// <summary>
    /// Obtém um usuário pelo nome.
    /// </summary>
    /// <param name="userName">O nome do usuário a ser obtido.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é o usuário com o nome especificado, ou nulo se nenhum usuário for encontrado.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o nome do usuário é nulo ou vazio.</exception>
    public async Task<User?> GetUserByNameAsync(string userName)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(userName, nameof(userName));

        var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

        var user = users.FirstOrDefault();

        if (user != null)
        {
            user.UserPassword = string.Empty;
        }

        return user;
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
    /// <exception cref="ExistingUserException">Lançada quando um usuário com o mesmo nome já existe.</exception>
    public async Task<User?> UpdateUserAsync(string userName, User updatedUser)
    {
        UserValidationHelper.ValidateUser(updatedUser);

        var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

        if (!users.Any())
        {
            return null;
        }

        var user = users.First();

        updatedUser.UserName = updatedUser.UserName.Trim();
        updatedUser.UserPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.UserPassword);

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

        user.UserPassword = string.Empty;

        return user;
    }
}
