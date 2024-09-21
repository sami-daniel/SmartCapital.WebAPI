using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI.Application.Implementations;

/// <summary>
/// Provides the implementation of user-related services, including CRUD operations and filtering.
/// </summary>
public class LoginService : ILoginService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginService"/> class with the provided unit of work.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used to manage repository operations and transactions.</param>
    public LoginService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User?> GetUserAsync(string userName, string pwd)
    {
        ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));

        var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

        var user = users.FirstOrDefault(u => u.UserName == userName && BCrypt.Net.BCrypt.Verify(pwd, u.UserPassword));

        if (user != null)
        {
            user.UserPassword = string.Empty; // Do not expose the user's password.
        }

        return user;
    }
}
