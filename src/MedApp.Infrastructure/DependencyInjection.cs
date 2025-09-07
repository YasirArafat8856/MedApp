using MedApp.Application.Interfaces;
using MedApp.Infrastructure.Email;
using MedApp.Infrastructure.Persistence;
using MedApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection") ?? "Server=localhost;Database=MedApp;Trusted_Connection=True;TrustServerCertificate=True;";
        services.AddDbContext<MedAppDbContext>(opt => opt.UseSqlServer(cs));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}
