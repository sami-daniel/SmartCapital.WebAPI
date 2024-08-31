// none

using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI.Application.Implementations
{
    /// <summary>
    /// Fornece a implementa��o dos servi�os relacionados a perfis, incluindo opera��es CRUD e filtragem.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Inicializa uma nova inst�ncia da classe <see cref="ProfileService"/> com a unidade de trabalho fornecida.
        /// </summary>
        /// <param name="unitOfWork">A unidade de trabalho usada para gerenciar opera��es de reposit�rio e transa��es.</param>
        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adiciona um novo perfil ao sistema.
        /// </summary>
        /// <param name="profileToAdd">O perfil a ser adicionado.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona.</returns>
        /// <exception cref="ArgumentNullException">Lan�ada quando o perfil a ser adicionado � nulo.</exception>
        /// <exception cref="ArgumentException">Lan�ada quando o nome do perfil � inv�lido.</exception>
        /// <exception cref="ExistingProfileException">Lan�ada quando um perfil com o mesmo nome j� existe.</exception>
        public async Task AddProfileAsync(Profile profileToAdd, string userName)
        {
            ArgumentNullException.ThrowIfNull(profileToAdd, nameof(profileToAdd));

            ArgumentException.ThrowIfNullOrEmpty(userName, nameof(userName));

            ArgumentException.ThrowIfNullOrEmpty(profileToAdd.ProfileName, nameof(profileToAdd.ProfileName));

            if (profileToAdd.ProfileName.Length > 255)
                throw new ArgumentException("O tamanho do Nome do Perfil n�o pode exceder 255 caracteres.");

            if (!Regex.Match(profileToAdd.ProfileName, "^[a-zA-Z0-9 ]*$").Success)
            {
                throw new ArgumentException("O Nome do Perfil pode conter somente letras, n�meros e espa�os.");
            }

            if (profileToAdd.ProfileOpeningBalance != null)
            {
                if (profileToAdd.ProfileOpeningBalance > 999_999_999.99m)
                    throw new ArgumentException("O tamanho do Saldo Inicial do Perfil n�o pode ser maior que 999.999.999,99.");
            }

            profileToAdd.ProfileName = profileToAdd.ProfileName.Trim();

            var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName, includeProperties: "Profiles");

            var user = users.FirstOrDefault();

            if (user == null)
                throw new ArgumentException("O Usu�rio que est� adicionando o Perfil n�o foi encontrado.");

            if (user.Profiles.Any(p => p.ProfileName == profileToAdd.ProfileName))
                throw new ExistingProfileException($"Um Perfil com o nome {profileToAdd.ProfileName} j� existe.");

            using (var transaction = await _unitOfWork.StartTransactionAsync())
            {
                try
                {
                    user.Profiles.Add(profileToAdd);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();
                    throw new ExistingProfileException($"Um Perfil com o nome {profileToAdd.ProfileName} j� existe.");
                }
            }
        }

        /// <summary>
        /// Obt�m todos os perfis do sistema.
        /// </summary>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � uma cole��o de todos os perfis.</returns>
        public async Task<IEnumerable<Profile>> GetAllProfilesAsync(Expression<Func<Profile, bool>>? filter = null,
                                                                    Func<IQueryable<Profile>, IOrderedQueryable<Profile>>? orderBy = null,
                                                                    string includeProperties = "")
        {
            return await _unitOfWork.ProfileRepository.GetAsync(filter, orderBy, includeProperties);
        }

        /// <summary>
        /// Obt�m um perfil pelo nome.
        /// </summary>
        /// <param name="profileName">O nome do perfil a ser obtido.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � o perfil com o nome especificado, ou nulo se nenhum perfil for encontrado.</returns>
        /// <exception cref="ArgumentException">Lan�ada quando o nome do perfil � nulo ou vazio.</exception>
        public async Task<Profile?> GetProfileByNameAsync(string profileName)
        {
            ArgumentException.ThrowIfNullOrEmpty(profileName, nameof(profileName));

            var profiles = await _unitOfWork.ProfileRepository.GetAsync(p => p.ProfileName == profileName);

            return profiles.FirstOrDefault();
        }

        /// <summary>
        /// Remove um perfil existente do sistema.
        /// </summary>
        /// <param name="profileToRemove">O perfil a ser removido.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona.</returns>
        /// <exception cref="ArgumentNullException">Lan�ada quando o perfil a ser removido � nulo.</exception>
        public async Task RemoveProfileAsync(Profile profileToRemove)
        {
            ArgumentNullException.ThrowIfNull(profileToRemove, "O Perfil a Remover n�o pode ser nulo.");

            _unitOfWork.ProfileRepository.Delete(profileToRemove);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Atualiza um perfil existente no sistema.
        /// </summary>
        /// <param name="profileName">O nome do perfil a ser atualizado.</param>
        /// <param name="updatedProfile">O objeto perfil atualizado.</param>
        /// <returns>Uma tarefa que representa a opera��o ass�ncrona. O resultado � o objeto perfil atualizado, ou nulo se o perfil n�o for encontrado.</returns>
        /// <exception cref="ArgumentException">Lan�ada quando o nome do perfil ou o perfil atualizado � nulo ou inv�lido.</exception>
        /// <exception cref="ExistingProfileException">Lan�ada quando um perfil com o mesmo nome j� existe.</exception>
        public async Task<Profile?> UpdateProfileAsync(string profileName, Profile updatedProfile)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(profileName, nameof(profileName));

            ArgumentNullException.ThrowIfNull(updatedProfile, nameof(updatedProfile));

            ArgumentException.ThrowIfNullOrEmpty(updatedProfile.ProfileName, nameof(updatedProfile.ProfileName));

            var profiles = await _unitOfWork.ProfileRepository.GetAsync(p => p.ProfileName == profileName);

            if (!profiles.Any())
            {
                return null;
            }

            var profile = profiles.First();

            if (profile.ProfileName.Length > 255)
                throw new ArgumentException("O tamanho do Nome do Perfil n�o pode exceder 255 caracteres.");

            if (!Regex.Match(profile.ProfileName, "^[a-zA-Z0-9 ]*$").Success)
            {
                throw new ArgumentException("O Nome do Perfil pode conter somente letras, n�meros e espa�os.");
            }

            if (profile.ProfileOpeningBalance != null)
            {
                if (profile.ProfileOpeningBalance > 999_999_999.99m)
                    throw new ArgumentException("O tamanho do Saldo Inicial do Perfil n�o pode ser maior que 999.999.999,99.");
            }

            updatedProfile.ProfileName = updatedProfile.ProfileName.Trim();

            profile.ProfileName = updatedProfile.ProfileName;
            profile.ProfileOpeningBalance = updatedProfile.ProfileOpeningBalance;

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
                    throw new ExistingProfileException($"Um Perfil com o nome {profile.ProfileName} j� existe.");
                }
            }

            return profile;
        }
    }
}
