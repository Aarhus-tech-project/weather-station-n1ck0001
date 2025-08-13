using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Persistence;

public interface IWeatherContext
{
    public DbSet<WeatherData> Data { get; set; }


    // required? 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}