using Application.Common.Interfaces.Factories.Entites;
using Application.Common.Interfaces.Factories.Results;
using Application.Common.Interfaces.Persistence;
using Application.Common.Results;
using Application.Weather.Dtos;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.Weather.Commands.Create;

public record CreateWeatherCommand(float Temp, float Humidity, float Pressure) : IRequest<Result<WeatherDto>>;

public class CreateWeatherCommandHandler(
    IWeatherContext weatherContext,
    IMapper mapper,
    IValidator<CreateWeatherCommand> validator,
    IResultFactory resultFactory,
    IWeatherDataFactory weatherDataFactory)
    : IRequestHandler<CreateWeatherCommand, Result<WeatherDto>>
{
    public async Task<Result<WeatherDto>> Handle(CreateWeatherCommand request, CancellationToken cancellationToken)
    {
        var isValid = await validator.ValidateAsync(request, cancellationToken);

        if (!isValid.IsValid) return resultFactory.BadRequest<WeatherDto>();

        var newData = weatherDataFactory.CreateWeatherData(request);

        await weatherContext.Data.AddAsync(newData, cancellationToken);
        await weatherContext.SaveChangesAsync(cancellationToken);

        return resultFactory.Created(mapper.Map<WeatherDto>(newData));
    }

    public class CreateWeatherCommandValidator : AbstractValidator<CreateWeatherCommand>
    {
        public CreateWeatherCommandValidator()
        {
            RuleFor(b => b.Temp).NotEmpty();
            RuleFor(b => b.Humidity).NotEmpty();
            RuleFor(b => b.Pressure).NotEmpty();
        }
    }
}