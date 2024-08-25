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
    /// Fornece a implementação dos serviços relacionados a perfis, incluindo operações CRUD e filtragem.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ProfileService"/> com a unidade de trabalho fornecida.
        /// </summary>
        /// <param name="unitOfWork">A unidade de trabalho usada para gerenciar operações de repositório e transações.</param>
        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddProfileAsync(Profile profileToAdd)
        {
            ArgumentNullException.ThrowIfNull(profileToAdd, "O Perfil a ser adicionado não pode ser nulo.");

            ArgumentException.ThrowIfNullOrEmpty(profileToAdd.ProfileName, "O Nome do Perfil não pode ser vazio ou nulo.");

            if (profileToAdd.ProfileName.Length > 255)
                throw new ArgumentException("O tamanho do Nome do Perfil não pode exceder 255 caracteres.");

            if (!Regex.Match(profileToAdd.ProfileName, "^[a-zA-Z0-9 ]*$").Success)
            {
                throw new ArgumentException("O Nome do Perfil pode conter somente letras, números e espaços.");
            }

            if (profileToAdd.ProfileOpeningBalance != null)
            {
                if (profileToAdd.ProfileOpeningBalance > 999_999_999.99m)
                    throw new ArgumentException("O tamanho do Saldo Inicial do Perfil não pode ser maior que 999.999.999,99.");
            }

            profileToAdd.ProfileName = profileToAdd.ProfileName.Trim();

            using (var transaction = await _unitOfWork.StartTransactionAsync()) 
            {
                try
                {
                    await _unitOfWork.ProfileRepository.InsertAsync(profileToAdd);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();
                    throw new ExistingProfileException($"Um Perfil com o nome {profileToAdd.ProfileName} já existe.");
                }
            }
        }

        public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
        {
            return await _unitOfWork.ProfileRepository.GetAsync();
        }

        public Task<IEnumerable<Profile>> GetFilteredProfilesAsync(Expression<Func<Profile, bool>> filter)
        {
            return _unitOfWork.ProfileRepository.GetAsync(filter: filter);
        }

        public async Task<Profile?> GetProfileByNameAsync(string profileName)
        {
            var profiles = await _unitOfWork.ProfileRepository.GetAsync(p => p.ProfileName == profileName);

            if (!profiles.Any())
            {
                return null;
            }

            return profiles.First();
        }

        public async Task RemoveProfileAsync(Profile profileToRemove)
        {
            ArgumentNullException.ThrowIfNull(profileToRemove, "O Perfil a Remover não pode ser nulo.");

            _unitOfWork.ProfileRepository.Delete(profileToRemove);
            await _unitOfWork.CompleteAsync();
        }
    }
}
