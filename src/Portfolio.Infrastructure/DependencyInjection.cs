using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Common.Interfaces;
using Portfolio.Infrastructure.Files;
using Portfolio.Infrastructure.Security;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IFileStorageService, FileStorageService>();

        return services;
    }
}
