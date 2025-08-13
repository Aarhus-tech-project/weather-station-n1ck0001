using Application.Common.Interfaces.Factories.Results;
using Application.Common.Interfaces.Persistence;
using Application.Common.Results;
using Application.Weather.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Weather.Queries;

public record GetWeatherDataQuery : IRequest<Result<List<WeatherDto>>>;

public class GetWeatherDataQueryHandler(IWeatherContext weatherContext, IMapper mapper, IResultFactory resultFactory)
    : IRequestHandler<GetWeatherDataQuery, Result<List<WeatherDto>>>
{
    public async Task<Result<List<WeatherDto>>> Handle(GetWeatherDataQuery request, CancellationToken cancellationToken)
    {
        var weatherData = await weatherContext.Data.Take(30).OrderByDescending(d => d.Reading_Time)
            .ToListAsync(cancellationToken);

        if (weatherData.Count == 0) return resultFactory.NotFound<List<WeatherDto>>();

        var ItemsToReturn = new List<WeatherDto>();

        foreach (var item in weatherData) ItemsToReturn.Add(mapper.Map<WeatherDto>(item));

        return resultFactory.Ok(ItemsToReturn);
    }
}