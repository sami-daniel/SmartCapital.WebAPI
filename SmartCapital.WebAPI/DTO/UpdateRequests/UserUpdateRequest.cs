using SmartCapital.WebAPI.Domain.Domain;
using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.DTO.UpdateRequests
{
    public class UserUpdateRequest
    {
        [Required(ErrorMessage = "O Nome do Usuário não pode ser vazio.")]
        [StringLength(255, ErrorMessage = "O tamanho do Nome do Usuário não pode exceder {0} caracteres")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "O Nome do Usuário pode conter somente letras e números.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "A senha não pode ser vazia.")]
        public string UserPassword { get; set; } = null!;

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
