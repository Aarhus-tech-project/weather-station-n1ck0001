using _2WeekCaseApi.Common.Interfaces.Factories;
using Application.Weather.Commands.Create;
using Application.Weather.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace _2WeekCaseApi.Controllers.Weather;

[Route("api/weather")]
[ApiController]
public class WeatherController(IMediator mediator, IResponseFactory responseFactory) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateWeatherData([FromBody] CreateWeatherCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return responseFactory.CreateResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetWeatherData()
    {
        var result = await mediator.Send(new GetWeatherDataQuery());
        return responseFactory.CreateResponse(result);
    }
}