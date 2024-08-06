using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI.Application.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;

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

            if (profileToAdd.ProfileOpeningBalance != null)
            {
                if (profileToAdd.ProfileOpeningBalance > 999_999_999.99m)
                    throw new ArgumentException("O tamanho do Saldo Inicial do Perfil não pode ser maior que 999.999.999,99.");
            }

            try
            {
                await _unitOfWork.ProfileRepository.InsertAsync(profileToAdd);
            }
            catch (DbUpdateException)
            {
                throw new ArgumentException($"Um Perfil com o nome {profileToAdd.ProfileName} já existe.");
            }
        }

        public async Task RemoveProfileAsync(Profile profileToRemove)
        {
            ArgumentNullException.ThrowIfNull(profileToRemove, "O Perfil a Remover não pode ser nulo.");

            await _unitOfWork.ProfileRepository.DeleteAsync(profileToRemove);
        }
    }
}
