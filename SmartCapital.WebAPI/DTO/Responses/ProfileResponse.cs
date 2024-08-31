// none

using SmartCapital.WebAPI.Domain.Domain;

namespace SmartCapital.WebAPI.DTO.Responses
{
    /// <summary>
    /// Representa a resposta que contém os detalhes de um perfil.
    /// </summary>
    public class ProfileResponse
    {
        /// <summary>
        /// Data de criação do perfil.
        /// </summary>
        public DateTime ProfileCreationDate { get; set; }

        /// <summary>
        /// Nome do perfil.
        /// </summary>
        /// <remarks>
        /// Este campo nunca deve ser nulo.
        /// </remarks>
        public string ProfileName { get; set; } = null!;

        /// <summary>
        /// Saldo inicial do perfil.
        /// </summary>
        /// <remarks>
        /// Este campo é opcional e pode ser nulo.
        /// </remarks>
        public decimal? ProfileOpeningBalance { get; set; }

        /// <summary>
        /// Usuário associado ao perfil.
        /// </summary>
        public UserResponse User { get; set; } = null!;
    }

    /// <summary>
    /// Contém métodos de extensão para a classe <see cref="Profile"/>.
    /// </summary>
    public static class ProfileExtensions
    {
        /// <summary>
        /// Converte uma instância de <see cref="Profile"/> em uma instância de <see cref="ProfileResponse"/>.
        /// </summary>
        /// <param name="profile">A instância de <see cref="Profile"/> a ser convertida.</param>
        /// <returns>Uma nova instância de <see cref="ProfileResponse"/> com os dados do perfil.</returns>
        public static ProfileResponse ToProfileResponse(this Profile profile)
        {
            return new ProfileResponse
            {
                ProfileCreationDate = profile.ProfileCreationDate,
                ProfileOpeningBalance = profile.ProfileOpeningBalance,
                ProfileName = profile.ProfileName,
                User = profile.UsersUser.ToUserResponse()
            };
        }
    }
}
