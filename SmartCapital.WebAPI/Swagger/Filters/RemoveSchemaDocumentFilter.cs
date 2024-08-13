using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmartCapital.WebAPI.Swagger.Filters
{
    public class RemoveSchemaDocumentFilter : IDocumentFilter
    {
        private readonly string _schemaNameToRemove;

        public RemoveSchemaDocumentFilter(string schemaNameToRemove)
        {
            _schemaNameToRemove = schemaNameToRemove;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc.Components.Schemas.ContainsKey(_schemaNameToRemove))
            {
                swaggerDoc.Components.Schemas.Remove(_schemaNameToRemove);
            }
        }
    }
}
