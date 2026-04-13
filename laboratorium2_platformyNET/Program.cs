using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var client = new HttpClient();
        string apiKey = "5dc91c85f046bd0d6519a200768e3036";

        using var db = new WeatherContext();

        Console.WriteLine("=== WeatherApp z bazą danych ===");
        Console.WriteLine("Komendy: <miasto> | list | sort | filter <min_temp> | delete <miasto> | exit");
        Console.WriteLine();

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;


            if (input.ToLower() == "exit")
                break;


            if (input.ToLower() == "list")
            {
                var records = db.WeatherRecords.Include(r => r.City).ToList();
                if (records.Count == 0)
                    Console.WriteLine("Baza danych jest pusta.");
                else
                    records.ForEach(r => Console.WriteLine(r));
                continue;
            }


            if (input.ToLower() == "sort")
            {
                var sorted = db.WeatherRecords
                    .Include(r => r.City)
                    .OrderByDescending(r => r.Temperature)
                    .ToList();

                if (sorted.Count == 0)
                    Console.WriteLine("Brak rekordów w bazie.");
                else
                {
                    Console.WriteLine("Posortowane malejąco wg temperatury:");
                    sorted.ForEach(r => Console.WriteLine(r));
                }
                continue;
            }


            if (input.ToLower().StartsWith("filter "))
            {
                string[] parts = input.Split(' ');
                if (parts.Length < 2 || !float.TryParse(parts[1], out float minTemp))
                {
                    Console.WriteLine("Użycie: filter <minimalna_temperatura>");
                    continue;
                }

                var filtered = db.WeatherRecords
                    .Include(r => r.City)
                    .Where(r => r.Temperature >= minTemp)
                    .ToList();

                if (filtered.Count == 0)
                    Console.WriteLine($"Brak rekordów z temperaturą >= {minTemp}°C.");
                else
                    filtered.ForEach(r => Console.WriteLine(r));
                continue;
            }


            if (input.ToLower().StartsWith("delete "))
            {
                string cityName = input.Substring(7).Trim();
                var city = db.Cities
                    .Include(c => c.WeatherRecords)
                    .FirstOrDefault(c => c.Name.ToLower() == cityName.ToLower());

                if (city == null)
                {
                    Console.WriteLine($"Nie znaleziono miasta '{cityName}' w bazie.");
                    continue;
                }

                db.WeatherRecords.RemoveRange(city.WeatherRecords);
                db.Cities.Remove(city);
                db.SaveChanges();
                Console.WriteLine($"Usunięto wszystkie rekordy dla miasta '{city.Name}'.");
                continue;
            }


            string requestedCity = input;
            var existingCity = db.Cities
                .Include(c => c.WeatherRecords)
                .FirstOrDefault(c => c.Name.ToLower() == requestedCity.ToLower());

            if (existingCity != null && existingCity.WeatherRecords.Count > 0)
            {
                // Dane są już w bazie — nie odpytujemy API
                var latest = existingCity.WeatherRecords
                    .OrderByDescending(r => r.FetchedAt)
                    .First();

                Console.WriteLine("[Z bazy danych]");
                Console.WriteLine($"Miasto: {existingCity.Name}");
                Console.WriteLine($"Temperatura: {latest.Temperature} C");
                Console.WriteLine($"Cisnienie: {latest.Pressure} mBar");
                Console.WriteLine($"Wilgotnosc: {latest.Humidity} %");
                Console.WriteLine($"Opis: {latest.Description}");
                Console.WriteLine($"Pobrano: {latest.FetchedAt:g}");
            }
            else
            {

                string url = $"https://api.openweathermap.org/data/2.5/weather" +
                             $"?q={requestedCity}&appid={apiKey}&units=metric&lang=pl";
                try
                {
                    string response = await client.GetStringAsync(url);
                    var weather = JsonSerializer.Deserialize<WeatherResponse>(response);

                    if (weather == null) { Console.WriteLine("Błąd parsowania odpowiedzi."); continue; }


                    if (existingCity == null)
                    {
                        existingCity = new City { Name = weather.name };
                        db.Cities.Add(existingCity);
                        db.SaveChanges(); // żeby CityId był dostępny
                    }


                    var record = new WeatherRecord
                    {
                        CityId = existingCity.CityId,
                        Temperature = weather.main.temp,
                        Pressure = weather.main.pressure,
                        Humidity = weather.main.humidity,
                        Description = weather.weather[0].description,
                        FetchedAt = DateTime.Now
                    };

                    db.WeatherRecords.Add(record);
                    db.SaveChanges();

                    Console.WriteLine("[Z API — zapisano do bazy]");
                    Console.WriteLine($"Miasto: {weather.name}");
                    Console.WriteLine($"Temperatura: {weather.main.temp} C");
                    Console.WriteLine($"Cisnienie: {weather.main.pressure} mBar");
                    Console.WriteLine($"Wilgotnosc: {weather.main.humidity} %");
                    Console.WriteLine($"Opis: {weather.weather[0].description}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Błąd: " + e.Message);
                }
            }
        }
    }
}


public class WeatherResponse
{
    public Main main { get; set; }
    public Weather[] weather { get; set; }
    public string name { get; set; }
}
public class Main
{
    public float temp { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
}
public class Weather
{
    public string description { get; set; }
}
