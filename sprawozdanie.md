# Sprawozdanie — WeatherApp z bazą danych


Program jest konsolową aplikacją pogodową napisaną w języku C# (.NET). Umożliwia użytkownikowi sprawdzanie aktualnej pogody dla wybranego miasta, korzystając z publicznego API OpenWeatherMap.

wykorzystane technologie:

- **C# / .NET** — język i platforma aplikacji
- **Entity Framework Core** — ORM do obsługi bazy danych (metody `Include`, `SaveChanges`, klasy `DbContext`)
- **HttpClient** — asynchroniczne wysyłanie żądań HTTP do API pogodowego
- **System.Text.Json** — deserializacja odpowiedzi JSON z API
- **OpenWeatherMap API** — zewnętrzne źródło danych pogodowych (temperatura, ciśnienie, wilgotność, opis)

# Model bazodanowy:

- **City** — reprezentuje miasto (pola: `CityId`, `Name`, kolekcja `WeatherRecords`)
- **WeatherRecord** — pojedynczy odczyt pogodowy (pola: `CityId`, `Temperature`, `Pressure`, `Humidity`, `Description`, `FetchedAt`)

Do mapowania odpowiedzi z API służą trzy klasy pomocnicze:

- `WeatherResponse` — zawiera obiekt `Main`, tablicę `Weather[]` oraz nazwę miasta (`name`)
- `Main` — temperatura (`temp`), ciśnienie (`pressure`), wilgotność (`humidity`)
- `Weather` — opis słowny pogody (`description`)

## Opis działania

Pobieranie pogody

Po wpisaniu nazwy miasta program sprawdza, czy dane dla tego miasta istnieją już w bazie. Jeśli tak — wyświetla najnowszy zapisany rekord (oznaczony etykietą `[Z bazy danych]`). Jeśli nie — wysyła asynchroniczne zapytanie HTTP do API OpenWeatherMap, deserializuje odpowiedź JSON, zapisuje nowe miasto i rekord pogodowy w bazie, a następnie wyświetla wynik (etykieta `[Z API — zapisano do bazy]`).

## Obsługa błędów

Zapytanie do API jest otoczone blokiem `try-catch`. W przypadku błędu sieci, nieprawidłowej nazwy miasta lub problemów z deserializacją użytkownik otrzymuje komunikat z treścią wyjątku. Dodatkowo program sprawdza, czy wynik deserializacji nie jest `null`.
