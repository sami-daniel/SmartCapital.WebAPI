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
/// Provides the implementation of user-related services, including CRUD operations and filtering.
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class with the provided unit of work.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used to manage repository operations and transactions.</param>
    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Adds a new user to the system.
    /// </summary>
    /// <param name="userToAdd">The user to be added.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ExistingUserException">Thrown when a user with the same name already exists.</exception>
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
                throw new ExistingUserException($"A user with the name {userToAdd.UserName} already exists.");
            }
        }
    }

    /// <summary>
    /// Gets all users from the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The result is a collection of all users.</returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null, string includeProperties = "")
    {
        var users = await _unitOfWork.UserRepository.GetAsync(filter, orderBy, includeProperties);

        foreach (var user in users)
        {
            user.UserPassword = string.Empty;
        }

        return users;
    }

    /// <summary>
    /// Gets a user by name.
    /// </summary>
    /// <param name="userName">The name of the user to be obtained.</param>
    /// <returns>A task that represents the asynchronous operation. The result is the user with the specified name, or null if no user is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the username is null or empty.</exception>
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
    /// Removes an existing user from the system.
    /// </summary>
    /// <param name="userToRemove">The user to be removed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the user to be removed is null.</exception>
    public async Task RemoveUserAsync(User userToRemove)
    {
        ArgumentNullException.ThrowIfNull(userToRemove, "The user to be removed cannot be null.");

        _unitOfWork.UserRepository.Delete(userToRemove);
        await _unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Updates an existing user in the system.
    /// </summary>
    /// <param name="userName">The name of the user to be updated.</param>
    /// <param name="updatedUser">The updated user object.</param>
    /// <returns>A task that represents the asynchronous operation. The result is the updated user object, or null if the user is not found.</returns>
    /// <exception cref="ExistingUserException">Thrown when a user with the same name already exists.</exception>
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
                throw new ExistingUserException($"A user with the name {user.UserName} already exists.");
            }
        }

        user.UserPassword = string.Empty;

        return user;
    }
}
