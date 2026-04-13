using System;

public class City
{
    public int CityId { get; set; }
    public required string Name { get; set; }
    public string? Country { get; set; }

    public List<WeatherRecord> WeatherRecords { get; set; } = new();

    public override string ToString() =>
        $"[{CityId}] {Name}{(Country != null ? $", {Country}" : "")}";
}