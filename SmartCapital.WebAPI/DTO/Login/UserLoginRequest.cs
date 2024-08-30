// none

using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.DTO.Login
{
    /// <summary>
    /// Representa a solicitação de login do usuário, contendo as credenciais necessárias para autenticação.
    /// </summary>
    public class UserLoginRequest
    {
        /// <summary>
        /// O nome do usuário usado para autenticação. Deve ser fornecido e não pode estar vazio.
        /// </summary>
        /// <example>user@example.com</example>
        [Required(ErrorMessage = "O Nome do Usuário não pode ser vazio.")]
        [StringLength(255, ErrorMessage = "O Nome do Usuário não pode exceder {0} caracteres.")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// A senha do usuário usada para autenticação. Deve ser fornecida e não pode estar vazia.
        /// </summary>
        /// <example>yourpassword123</example>
        [Required(ErrorMessage = "A Senha do Usuário não pode ser vazia.")]
        [StringLength(255, ErrorMessage = "A Senha do Usuário não pode exceder {0} caracteres.")]
        public string UserPassword { get; set; } = null!;
    }
}
