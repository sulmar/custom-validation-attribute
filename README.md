# Własny atrybut walidacyjny GreaterThan

Prosty projekt pokazujący, jak utworzyć własny atrybut walidacyjny w C#, który porównuje wartości dwóch właściwości tego samego obiektu. Atrybut `GreaterThan` sprawdza, czy wartość oznaczonej właściwości jest większa od wartości właściwości wskazanej przez nazwę, np. czy data końca jest późniejsza niż data początku.

```csharp
public DateTime StartDate { get; set; }

[GreaterThan(nameof(StartDate))]
public DateTime EndDate { get; set; }
```

Implementacja korzysta z `ValidationContext.ObjectInstance`, aby uzyskać dostęp do obiektu, oraz z refleksji, aby odczytać drugą właściwość. Porównanie wykonuje metoda `IComparable.CompareTo`. Wartości muszą mieć ten sam typ i implementować `IComparable`. Wartości `null` są pomijane — ich wymaganie można określić osobnym atrybutem `[Required]`.

Projekt zawiera atrybut oraz testy xUnit sprawdzające porównania liczb i dat, obsługę błędów konfiguracji, komunikaty walidacyjne i współpracę z `Validator.TryValidateObject`.

## Uruchomienie

Wymagany jest .NET 10 SDK. W katalogu `GreaterThan.Tests` uruchom:

```bash
dotnet test
```
