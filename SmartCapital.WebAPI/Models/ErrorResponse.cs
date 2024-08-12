namespace SmartCapital.WebAPI.Models
{
    /// <summary>
    /// Modelo usado como resposta para erros nos endpoints dos controladores.
    /// </summary>
    public class ErrorResponse
    {
        public required string ErrorType { get; set; }
        public required string Message { get; set; }
    }
}
