using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Application.Implementations;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Implementations;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Implementations;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseMySql(builder.Configuration["ConnectionStrings:SmartCapitalDatabase"], new MySqlServerVersion(new Version(8, 4, 0))));

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
