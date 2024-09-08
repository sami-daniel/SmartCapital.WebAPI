using System.Text.RegularExpressions;
using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.Application.Helpers;
internal static class UserValidationHelper
{
    /// <summary>
    /// Valida um usuário antes de ser adicionado ao sistema.
    /// </summary>
    /// <param name="user">O usuário a ser validado.</param>
    /// <exception cref="ArgumentNullException">Lançada quando o usuário a ser adicionado é nulo.</exception>
    /// <exception cref="ArgumentException">Lançada quando o nome de usuário ou senha é inválido.</exception>
    internal static void ValidateUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        ArgumentException.ThrowIfNullOrWhiteSpace(user.UserName, nameof(user.UserName));
        ArgumentException.ThrowIfNullOrWhiteSpace(user.UserPassword, nameof(user.UserPassword));

        if (user.UserName.Length > 255)
            throw new ArgumentException("O tamanho do nome de usuário não pode exceder 255 caracteres.");

        if (!Regex.Match(user.UserName, "^[a-zA-Z0-9 ]*$").Success)
        {
            throw new ArgumentException("O nome de usuário pode conter apenas letras, números e espaços.");
        }

    }
}
