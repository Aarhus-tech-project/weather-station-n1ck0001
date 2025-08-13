using Application.Common.Interfaces.Factories.Entites;
using Application.Common.Interfaces.Factories.Results;
using Infrastructure.InterfaceImplementations.Factories.EntityFactories;
using Infrastructure.InterfaceImplementations.Factories.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class InterfaceExtensions
{
    public static IServiceCollection AddInterfaceImplementations(this IServiceCollection services)
    {
        services.AddScoped<IResultFactory, ResultFactory>();

        services.AddScoped<IWeatherDataFactory, WeatherDataFactory>();

        return services;
    }
}