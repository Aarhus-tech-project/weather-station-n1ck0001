using Application.Common.Interfaces.Persistence;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditEntitySaveChangesInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<WeatherContext>((provider, options) =>
        {
            var interceptor = provider.GetRequiredService<AuditEntitySaveChangesInterceptor>();

            options
                .UseSqlServer(connectionString)
                .AddInterceptors(interceptor);
        });

        services.AddScoped<IWeatherContext>(provider => provider.GetRequiredService<WeatherContext>());

        return services;
    }
}