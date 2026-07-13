using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Interfaces.Repositories;
using Portfolio.Application.Interfaces.Identity;
using Portfolio.Application.Interfaces.Storage;
using Portfolio.Infrastructure.Authentication;
using Portfolio.Infrastructure.FileStorage;
using Portfolio.Infrastructure.Persistence.Context;
using Portfolio.Infrastructure.Persistence.Repositories;
using Portfolio.Infrastructure.Persistence.UnitOfWork;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Infrastructure Services
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IFileStorageService, FileStorageService>();
        services.AddTransient<Application.Interfaces.Services.IEmailService, Email.EmailService>();
        services.AddTransient<Application.Interfaces.Services.INotificationSender, SignalR.NotificationSender>();
        services.AddScoped<IIdentityService, Identity.IdentityService>();
        services.AddScoped<Application.Interfaces.Services.INotificationService, Application.Services.NotificationService>();

        // Persistence Services
        services.AddDbContext<PortfolioDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(PortfolioDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
