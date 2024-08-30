// none

namespace SmartCapital.WebAPI.DTO.Login
{
    /// <summary>
    /// Representa a resposta de autenticação do usuário, contendo informações sobre o usuário autenticado e o token JWT gerado.
    /// </summary>
    public class UserLoginResponse
    {
        /// <summary>
        /// O nome do usuário autenticado.
        /// </summary>
        /// <example>user@example.com</example>
        public string User { get; set; } = null!;

        /// <summary>
        /// O papel ou a função do usuário no sistema.
        /// </summary>
        /// <example>APIUser</example>
        public string Role { get; set; } = null!;

        /// <summary>
        /// O token JWT gerado para o usuário autenticado.
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string Token { get; set; } = null!;
    }
}
