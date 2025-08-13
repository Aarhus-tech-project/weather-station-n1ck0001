using System.Reflection;
using _2WeekCaseApi.Common.Factories;
using _2WeekCaseApi.Common.Interfaces.Factories;
using Application.Weather.Commands.Create;
using FluentValidation;

namespace _2WeekCaseApi.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration,
        WebApplicationBuilder builder)
    {
        // swagger auth 

        services.AddScoped<IResponseFactory, ResponseFactory>();

        services.AddValidatorsFromAssemblyContaining<CreateWeatherCommand>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateWeatherCommand).Assembly);
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}