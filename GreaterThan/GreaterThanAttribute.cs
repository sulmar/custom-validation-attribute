using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GreaterThan;

[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanAttribute : ValidationAttribute
{
    public string OtherProperty { get; }

    public GreaterThanAttribute(string otherProperty)
        : base("{0} musi być większe niż {1}.")
    {
        OtherProperty = otherProperty;
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        // 1. Wyszukanie właściwości po nazwie:
        var property = validationContext.ObjectType.GetProperty(OtherProperty, BindingFlags.Instance | BindingFlags.Public);

        if (property is null ||
            !property.CanRead ||
            property.GetIndexParameters().Length != 0)
        {
            throw new InvalidOperationException(
                $"Nie znaleziono czytelnej właściwości '{OtherProperty}'.");
        }

        // 2. Odczyt wartości tej właściwości z konkretnego obiektu:
        var otherValue = property.GetValue(validationContext.ObjectInstance);

        // Brak wartości obsługuje osobny atrybut [Required].
        if (value is null || otherValue is null)
            return ValidationResult.Success;

        if (value.GetType() != otherValue.GetType() ||
            value is not IComparable comparable)
        {
            throw new InvalidOperationException(
                "Porównywane wartości muszą mieć ten sam typ " +
                "i implementować IComparable.");
        }
        
        // 3. Porównanie wartości
        // porównanie nie używa refleksji — jest zwykłym wywołaniem metody interfejsu IComparable:
        if (comparable.CompareTo(otherValue) > 0) // ten warunek odpowiada if (reservation.EndDate > reservation.StartDate)
            return ValidationResult.Success;
        
        /*
     * Metoda CompareTo zwraca:
       - Liczbę dodatnią — EndDate jest większe niż StartDate.
       - Zero — wartości są równe.
       - Liczbę ujemną — EndDate jest mniejsze niż StartDate.
     */

        // 4. Odczyt atrybutu [Display] przypisanego do właściwości:
        var otherDisplayName = property
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName() ?? OtherProperty;

        var message = string.Format(
            ErrorMessageString,
            validationContext.DisplayName,
            otherDisplayName);

        var memberNames = validationContext.MemberName is { } memberName
            ? new[] { memberName }
            : Array.Empty<string>();

        return new ValidationResult(message, memberNames);
    }
}