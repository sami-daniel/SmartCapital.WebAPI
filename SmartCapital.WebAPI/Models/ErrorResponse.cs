using System.ComponentModel.DataAnnotations;

namespace SmartCapital.WebAPI.Models
{
    /// <summary>
    /// Model used as a response for errors in controller endpoints.
    /// </summary>
    public class ErrorResponse
    {
        [Required]
        public string ErrorType { get; set; } = null!; // Prefer null! to required modifier
        [Required]
        public string Message { get; set; } = null!;
    }
}
