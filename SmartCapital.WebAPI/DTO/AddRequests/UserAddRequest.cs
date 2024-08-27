using SmartCapital.WebAPI.Domain.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.DTO.AddRequests
{
    public class UserAddRequest
    {
        [Required(ErrorMessage = "O Nome do Usuário não pode ser vazio.")]
        [StringLength(255, ErrorMessage = "O Nome do Usuário não pode exceder {0} caracteres.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "A Senha do Usuário não pode ser vazia.")]
        [StringLength(255, ErrorMessage = "A Senha do Usuário não pode ser vazia.")]
        public string? UserPassword { get; set; }

        public User ToUser()
        {
            return new User
            {
                UserName = UserName,
                UserPassword = UserPassword
            };
        }
    }
}
