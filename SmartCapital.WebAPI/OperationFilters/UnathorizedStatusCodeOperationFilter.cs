using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmartCapital.WebAPI.OperationFilters;

public class UnathorizedStatusCodeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Acesso não permitido devido a falta de autenticação ou autenticação incorreta."
            });
        }
    }
}
