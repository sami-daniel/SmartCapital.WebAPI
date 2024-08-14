using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.Models
{
    /// <summary>
    /// Modelo usado como resposta para erros nos endpoints dos controladores.
    /// </summary>
    public class ErrorResponse
    {
        [Required]
        public required string ErrorType { get; set; }
        [Required]
        public required string Message { get; set; }
    }
}
