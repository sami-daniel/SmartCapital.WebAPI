using SmartCapital.WebAPI.Domain.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.Application.DTO.AddRequests
{
    /// <summary>
    /// Representa uma solicitação para adicionar um novo perfil.
    /// </summary>
    public class ProfileAddRequest
    {
        /// <summary>
        /// Nome do perfil. Deve ter no máximo 255 caracteres.
        /// </summary>
        /// <remarks>
        /// Este campo é obrigatório e não pode ser nulo.
        /// </remarks>
        [Required]
        [StringLength(255, ErrorMessage = "O tamanho do Nome do Perfil não pode exceder {0} caracteres")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "O Nome do Perfil pode conter somente letras e números.")]
        public string ProfileName { get; set; } = null!;

        /// <summary>
        /// Saldo inicial do perfil. Deve estar dentro do intervalo permitido de 0 a 999.999.999,99.
        /// </summary>
        /// <remarks>
        /// Este campo é opcional e pode ser nulo.
        /// </remarks>
        [Range(0d, 999_999_999.99d, ErrorMessage = "O tamanho do Saldo Inicial do Perfil não pode ser maior que {1}.")]
        public decimal? ProfileOpeningBalance { get; set; }

        /// <summary>
        /// Converte a instância atual de <see cref="ProfileAddRequest"/> em uma instância de <see cref="Profile"/>.
        /// </summary>
        /// <returns>Uma nova instância de <see cref="Profile"/> com os dados do perfil.</returns>
        public Profile ToProfile()
        {
            return new Profile
            {
                ProfileName = ProfileName,
                ProfileOpeningBalance = ProfileOpeningBalance
            };
        }
    }
}
