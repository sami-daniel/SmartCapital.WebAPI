using SmartCapital.WebAPI.Domain.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.Application.DTO.AddRequests
{
    public class ProfileAddRequest
    {
        [Required]
        [StringLength(255, ErrorMessage = "O tamanho do Nome do Perfil não pode exceder {0} caracteres")]
        public string ProfileName { get; set; } = null!;
        [Range(0d, 999_999_999.99d, ErrorMessage = "O tamanho do Saldo Inicial do Perfil não pode ser maior que {1}.")]
        public decimal? ProfileOpeningBalance { get; set; }

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
