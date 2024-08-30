// none

using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.Models
{
    /// <summary>
    /// Modelo usado como resposta para erros nos endpoints dos controladores.
    /// </summary>
    public class ErrorResponse
    {
        [Required]
        public string ErrorType { get; set; } = null!; // Prefer null! to required modifier
        [Required]
        public string Message { get; set; } = null!;
    }
}
