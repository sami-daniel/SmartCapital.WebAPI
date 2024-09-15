﻿// none

using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Helpers;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI.Application.Implementations;

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

    /// <summary>
    /// Adiciona um novo perfil ao sistema.
    /// </summary>
    /// <param name="profileToAdd">O perfil a ser adicionado.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o perfil a ser adicionado é nulo.</exception>
    /// <exception cref="ArgumentException">Lançada quando o nome do perfil é inválido.</exception>
    /// <exception cref="ExistingProfileException">Lançada quando um perfil com o mesmo nome já existe.</exception>
    public async Task AddProfileAsync(Profile profileToAdd, string userName)
    {
        ProfileValidationHelper.ValidateProfile(profileToAdd);

        profileToAdd.ProfileName = profileToAdd.ProfileName.Trim();

        var users = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == userName, includeProperties: "Profiles");

        var user = users.FirstOrDefault() ?? throw new ArgumentException("O Usuário que está adicionando o Perfil não foi encontrado.");
        if (user.Profiles.Any(p => p.ProfileName == profileToAdd.ProfileName))
        {
            // FIXME: Essa verificação deveria também ser implementada no banco de dados
            // para maior consistencia e integridade. A unica coisa que proteje o usuário
            // de adicionar um perfil com o mesmo nome é a aplicação, o que não é suficiente
            // e inconsistente.

            throw new ExistingProfileException($"Um Perfil com o nome {profileToAdd.ProfileName} já existe.");
        }

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
                throw new ExistingProfileException($"Um Perfil com o nome {profileToAdd.ProfileName} já existe.");
            }
        }
    }

    /// <summary>
    /// Obtém todos os perfis do sistema.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de todos os perfis.</returns>
    public async Task<IEnumerable<Profile>> GetAllProfilesAsync(Expression<Func<Profile, bool>>? filter = null,
                                                                Func<IQueryable<Profile>, IOrderedQueryable<Profile>>? orderBy = null,
                                                                string includeProperties = "")
    {
        return await _unitOfWork.ProfileRepository.GetAsync(filter, orderBy, includeProperties);
    }

    /// <summary>
    /// Obtém um perfil pelo nome.
    /// </summary>
    /// <param name="profileName">O nome do perfil a ser obtido.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é o perfil com o nome especificado, ou nulo se nenhum perfil for encontrado.</returns>
    /// <exception cref="ArgumentException">Lançada quando o nome do perfil é nulo ou vazio.</exception>
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
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    /// <exception cref="ArgumentNullException">Lançada quando o perfil a ser removido é nulo.</exception>
    public async Task RemoveProfileAsync(Profile profileToRemove)
    {
        ArgumentNullException.ThrowIfNull(profileToRemove, "O Perfil a Remover não pode ser nulo.");

        _unitOfWork.ProfileRepository.Delete(profileToRemove);
        await _unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Atualiza um perfil existente no sistema.
    /// </summary>
    /// <param name="profileName">O nome do perfil a ser atualizado.</param>
    /// <param name="updatedProfile">O objeto perfil atualizado.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é o objeto perfil atualizado, ou nulo se o perfil não for encontrado.</returns>
    /// <exception cref="ArgumentException">Lançada quando o nome do perfil ou o perfil atualizado é nulo ou inválido.</exception>
    /// <exception cref="ExistingProfileException">Lançada quando um perfil com o mesmo nome já existe.</exception>
    public async Task<Profile?> UpdateProfileAsync(string profileName, Profile updatedProfile)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(profileName, nameof(profileName));

        ProfileValidationHelper.ValidateProfile(updatedProfile);

        var profiles = await _unitOfWork.ProfileRepository.GetAsync(p => p.ProfileName == profileName);

        if (!profiles.Any())
        {
            return null;
        }

        var profile = profiles.First();

        if (profile.ProfileName.Length > 255)
        {
            throw new ArgumentException("O tamanho do Nome do Perfil não pode exceder 255 caracteres.");
        }

        if (!Regex.Match(profile.ProfileName, "^[a-zA-Z0-9 ]*$").Success)
        {
            throw new ArgumentException("O Nome do Perfil pode conter somente letras, números e espaços.");
        }

        if (profile.ProfileOpeningBalance != null)
        {
            if (profile.ProfileOpeningBalance > 999_999_999.99m)
            {
                throw new ArgumentException("O tamanho do Saldo Inicial do Perfil não pode ser maior que 999.999.999,99.");
            }
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
                throw new ExistingProfileException($"Um Perfil com o nome {profile.ProfileName} já existe.");
            }
        }

        return profile;
    }
}
