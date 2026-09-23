# Własne atrybuty walidacyjne do porównań

Prosty projekt pokazujący, jak utworzyć własne atrybuty walidacyjne w C#, które porównują wartości dwóch właściwości tego samego obiektu. Każdy atrybut sprawdza inny warunek:

- `GreaterThan` — wartość jest większa od wskazanej właściwości
- `GreaterThanOrEqualTo` — wartość jest większa lub równa
- `LessThan` — wartość jest mniejsza
- `LessThanOrEqualTo` — wartość jest mniejsza lub równa

Przykładem jest data końca późniejsza, taka sama albo wcześniejsza niż data początku.

```csharp
public DateTime StartDate { get; set; }

[GreaterThan(nameof(StartDate))]
public DateTime EndDate { get; set; }

[GreaterThanOrEqualTo(nameof(StartDate))]
public DateTime EndDateOrSame { get; set; }
```

```csharp
public DateTime EndDate { get; set; }

[LessThan(nameof(EndDate))]
public DateTime StartDate { get; set; }

[LessThanOrEqualTo(nameof(EndDate))]
public DateTime StartDateOrSame { get; set; }
```

Wszystkie atrybuty dziedziczą po `CompareAttribute`. Wspólna implementacja korzysta z `ValidationContext.ObjectInstance`, aby uzyskać dostęp do obiektu, oraz z refleksji, aby odczytać drugą właściwość. Porównanie wykonuje metoda `IComparable.CompareTo`, a atrybut pochodny decyduje, czy wynik ma być większy, mniejszy, większy lub równy albo mniejszy lub równy. Wartości muszą mieć ten sam typ i implementować `IComparable`. Wartości `null` są pomijane — ich wymaganie można określić osobnym atrybutem `[Required]`.

Projekt zawiera te cztery atrybuty oraz testy xUnit sprawdzające porównania liczb i dat, obsługę błędów konfiguracji, komunikaty walidacyjne i współpracę z `Validator.TryValidateObject`.

## Uruchomienie

Wymagany jest .NET 10 SDK. W katalogu `GreaterThan.Tests` uruchom:

```bash
dotnet test
```
