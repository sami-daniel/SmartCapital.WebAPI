using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmartCapital.WebAPI.OperationFilters;

/// <summary>
/// Adds a 401 Unauthorized response to the Swagger documentation if it is not already present.
/// </summary>
public class UnathorizedStatusCodeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Access not allowed due to lack of authentication or incorrect authentication."
            });
        }
    }
}
