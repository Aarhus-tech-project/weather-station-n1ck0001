using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class WeatherContext(DbContextOptions<WeatherContext> options) : DbContext(options), IWeatherContext
{
    public DbSet<WeatherData> Data { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}