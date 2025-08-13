namespace Application.Weather.Dtos;

public class WeatherDto
{
    public float Temperature { get; set; }
    public float Humidity { get; set; }
    public float Pressure { get; set; }
    public DateTimeOffset Reading_Time { get; set; }
}