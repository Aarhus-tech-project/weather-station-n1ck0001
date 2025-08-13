using Domain.Common;

namespace Domain.Entities;

public class WeatherData : AuditableEntity
{
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float Pressure { get; set; }
    public DateTimeOffset Reading_Time { get; set; }
}