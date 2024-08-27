using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.Models
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "O Nome do Usuário não pode ser vazio.")]
        [StringLength(255, ErrorMessage = "O Nome do Usuário não pode exceder {0} caracteres.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "A Senha do Usuário não pode ser vazia.")]
        [StringLength(255, ErrorMessage = "A Senha do Usuário não pode ser vazia.")]
        public string? UserPassword { get; set; }
    }
}
