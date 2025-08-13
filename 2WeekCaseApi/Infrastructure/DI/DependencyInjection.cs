using Infrastructure.DI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)

    {
        services.AddDatabase(configuration);

        // want logging here bruh

        services.AddInterfaceImplementations();

        return services;
    }
}