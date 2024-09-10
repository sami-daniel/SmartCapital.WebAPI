// none

using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartCapital.WebAPI.Application.Implementations;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.AutoMapperProfiles;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Implementations;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Implementations;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;
using SmartCapital.WebAPI.Middlewares;
using SmartCapital.WebAPI.OperationFilters;

namespace SmartCapital.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddCors();
        builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(opt =>
            {
                var key = Encoding.ASCII.GetBytes(builder.Configuration["JWTSettings:Secret"] ?? throw new InvalidOperationException("O token do JWT (secret) não está definido."));
                opt.RequireHttpsMetadata = true;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
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

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });


            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            opt.OperationFilter<UnathorizedStatusCodeOperationFilter>();

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddAutoMapper(opt =>
        {
            opt.AddProfile<UserProfile>();
        });

        // Registering services of DataAcess
        builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseMySql(builder.Configuration["ConnectionStrings:SmartCapitalDatabase"] ?? throw new InvalidOperationException("A string de conexão não esta definida."), new MySqlServerVersion(new Version(8, 4, 0))));
        builder.Services.AddScoped<DapperContext>();

        // Repostories and UnitOfWork
        builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<ILoginService, LoginService>();
        builder.Services.AddScoped<IUserService, UserService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors(opt => opt
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseMiddleware<LoadResourcesMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
