public class WeatherRecord
{
    public int WeatherRecordId { get; set; }

    public int CityId { get; set; }
    public City? City { get; set; }

    public float Temperature { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public required string Description { get; set; }
    public DateTime FetchedAt { get; set; }

    public override string ToString() =>
        $"Miasto: {City?.Name,-15} | {Temperature,6:F1}°C | {Pressure} mBar | " +
        $"{Humidity}% | {Description,-25} | {FetchedAt:g}";
}