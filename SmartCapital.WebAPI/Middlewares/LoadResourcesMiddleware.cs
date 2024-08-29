using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SmartCapital.WebAPI.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartCapital.WebAPI.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoadResourcesMiddleware
    {
        private readonly RequestDelegate _next;

        public LoadResourcesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var userService = httpContext.RequestServices.GetRequiredService<IUserService>();

            var token = httpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(' ').Last();

            if (token != null)
            {
                var jwtSecurity = new JwtSecurityTokenHandler();
                var securityToken = jwtSecurity.ReadJwtToken(token);
                var name = securityToken.Claims.First(claim => claim.Type == "unique_name").Value;
                
                httpContext.Items["User"] = name;
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoadResourcesMiddleware>();
        }
    }
}
