using Application.Weather.Commands.Create;
using Domain.Entities;

namespace Application.Common.Interfaces.Factories.Entites;

public interface IWeatherDataFactory
{
    public WeatherData CreateWeatherData(CreateWeatherCommand command);
}