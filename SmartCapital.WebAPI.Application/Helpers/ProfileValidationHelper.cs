using System.Text.RegularExpressions;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Helpers;
internal class ProfileValidationHelper
{
    /// <summary>
    /// Valida um perfil antes de ser adicionado ao sistema.
    /// </summary>
    /// <param name="profile">O perfil a ser validado.</param>
    /// <exception cref="ArgumentException">Excessão disparada se há algum erro de validação.</exception>
    internal static void ValidateProfile(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile, nameof(profile));

        ArgumentException.ThrowIfNullOrWhiteSpace(profile.ProfileName, nameof(profile));

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
    }
}
