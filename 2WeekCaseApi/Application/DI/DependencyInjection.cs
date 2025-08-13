using Application.Common.Mapperrr;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(
            cfg =>
            {
                // Optional global config, e.g., cfg.ForAllMaps(...);
            },
            typeof(MappingProfile).Assembly
        );

        return services;
    }
}