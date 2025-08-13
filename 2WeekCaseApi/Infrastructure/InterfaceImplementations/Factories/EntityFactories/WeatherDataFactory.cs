using Application.Common.Interfaces.Factories.Entites;
using Application.Weather.Commands.Create;
using Domain.Entities;

namespace Infrastructure.InterfaceImplementations.Factories.EntityFactories;

public class WeatherDataFactory : IWeatherDataFactory
{
    public WeatherData CreateWeatherData(CreateWeatherCommand command)
    {
        return new WeatherData
        {
            Humidity = command.Humidity,
            Pressure = command.Pressure,
            Temperature = command.Temp,
            Reading_Time = DateTimeOffset.UtcNow
        };
    }
}