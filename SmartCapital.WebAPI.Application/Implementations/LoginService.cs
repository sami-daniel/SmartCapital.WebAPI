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
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="LoginService"/> com a unidade de trabalho fornecida.
        /// </summary>
        /// <param name="unitOfWork">A unidade de trabalho usada para gerenciar operações de repositório e transações.</param>
        public LoginService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserAsync(string userName, string pwd)
        {
            ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));

            var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName);

            return users.FirstOrDefault(u => u.UserName == userName && u.UserPassword == pwd);
        }
    }
}
