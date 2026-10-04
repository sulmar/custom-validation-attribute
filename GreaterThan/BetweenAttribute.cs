using System.ComponentModel.DataAnnotations;

namespace GreaterThan;

// Wzorzec projektowy Template Method — analogicznie do CompareAttribute.
public abstract class BetweenAttribute : ValidationAttribute
{
    private readonly object from;
    private readonly object to;

    protected BetweenAttribute(object from, object to, string errorMessage)
        : base(errorMessage)
    {
        if (from is null || to is null)
        {
            throw new InvalidOperationException("Granice zakresu nie mogą być puste.");
        }

        if (from.GetType() != to.GetType() || from is not IComparable comparable)
        {
            throw new InvalidOperationException(
                "Granice zakresu muszą mieć ten sam typ i implementować IComparable.");
        }

        if (comparable.CompareTo(to) > 0)
        {
            throw new InvalidOperationException("Dolna granica nie może być większa od górnej.");
        }

        this.from = from;
        this.to = to;
    }

    protected abstract bool Predicate(int fromComparison, int toComparison);

    protected sealed override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Brak wartości obsługuje osobny atrybut [Required].
        if (value is null)
            return ValidationResult.Success;

        if (value.GetType() != from.GetType() || value is not IComparable comparable)
        {
            throw new InvalidOperationException(
                "Porównywana wartość musi mieć ten sam typ co granice zakresu i implementować IComparable.");
        }

        // CompareTo(from) > 0 oznacza wartość większą od dolnej granicy.
        // CompareTo(to) < 0 oznacza wartość mniejszą od górnej granicy.
        if (Predicate(comparable.CompareTo(from), comparable.CompareTo(to)))
            return ValidationResult.Success;

        var message = string.Format(
            ErrorMessageString,
            validationContext.DisplayName,
            from,
            to);

        var memberNames = validationContext.MemberName is { } memberName
            ? new[] { memberName }
            : Array.Empty<string>();

        return new ValidationResult(message, memberNames);
    }
}
