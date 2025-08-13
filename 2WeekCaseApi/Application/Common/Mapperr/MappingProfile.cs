using Application.Weather.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Mapperrr;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WeatherData, WeatherDto>();
    }
}