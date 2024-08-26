using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SmartCapital.WebAPI.Application.Implementations;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Implementations;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Implementations;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;
using System.Reflection;

namespace SmartCapital.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddRouting(opt =>
            {
                opt.LowercaseQueryStrings = true;
                opt.LowercaseUrls = true;
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SmartCapital Web API",
                    Description = "Uma Web API ASP.NET Core para controle financeiro, totalmente personalizavel"
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseMySql(builder.Configuration["ConnectionStrings:SmartCapitalDatabase"] ?? throw new InvalidOperationException("A string de conexão não esta definida."), new MySqlServerVersion(new Version(8, 4, 0))));

            builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IProfileService, ProfileService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
