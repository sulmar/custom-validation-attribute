# Własne atrybuty walidacyjne do porównań

Prosty projekt pokazujący, jak utworzyć własne atrybuty walidacyjne w C#, które porównują wartości dwóch właściwości tego samego obiektu. Każdy atrybut sprawdza inny warunek:

- `GreaterThan` — wartość jest większa od wskazanej właściwości
- `GreaterThanOrEqualTo` — wartość jest większa lub równa
- `LessThan` — wartość jest mniejsza
- `LessThanOrEqualTo` — wartość jest mniejsza lub równa
- `ExclusiveBetween` — wartość jest większa od dolnej granicy i mniejsza od górnej
- `InclusiveBetween` — wartość jest większa lub równa dolnej granicy i mniejsza lub równa górnej

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

Zakres względem dwóch stałych granic:

```csharp
[ExclusiveBetween(1, 10)]
public int Id { get; set; }

[InclusiveBetween(1, 10)]
public int Score { get; set; }
```

Atrybuty porównujące dwie właściwości dziedziczą po `CompareAttribute`. Wspólna implementacja korzysta z `ValidationContext.ObjectInstance`, aby uzyskać dostęp do obiektu, oraz z refleksji, aby odczytać drugą właściwość. Porównanie wykonuje metoda `IComparable.CompareTo`, a atrybut pochodny decyduje, czy wynik ma być większy, mniejszy, większy lub równy albo mniejszy lub równy.

`ExclusiveBetween` i `InclusiveBetween` korzystają z tego samego wzorca metody szablonowej w `BetweenAttribute`: klasa bazowa porównuje wartość z obiema granicami, a klasa pochodna nadpisuje predykat. Wartości muszą mieć ten sam typ i implementować `IComparable`. Wartości `null` są pomijane — ich wymaganie można określić osobnym atrybutem `[Required]`.

Projekt zawiera te atrybuty oraz testy xUnit sprawdzające porównania liczb i dat, obsługę błędów konfiguracji, komunikaty walidacyjne i współpracę z `Validator.TryValidateObject`.

## Uruchomienie

Wymagany jest .NET 10 SDK. W katalogu głównym repozytorium uruchom:

```bash
dotnet test
```
