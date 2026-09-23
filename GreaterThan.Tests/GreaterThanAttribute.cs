using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GreaterThan.Tests;

[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanAttribute : ValidationAttribute
{
    public string OtherProperty { get; }

    public GreaterThanAttribute(string otherProperty)
        : base("{0} musi być większe niż {1}.")
    {
        OtherProperty = otherProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(
            OtherProperty, BindingFlags.Instance | BindingFlags.Public);

        if (property is null || !property.CanRead || property.GetIndexParameters().Length != 0)
            throw new InvalidOperationException(
                $"Nie znaleziono czytelnej właściwości '{OtherProperty}'.");

        var otherValue = property.GetValue(validationContext.ObjectInstance);
        if (value is null || otherValue is null)
            return ValidationResult.Success;

        if (value.GetType() != otherValue.GetType() || value is not IComparable comparable)
            throw new InvalidOperationException(
                "Porównywane wartości muszą mieć ten sam typ i implementować IComparable.");

        if (comparable.CompareTo(otherValue) > 0)
            return ValidationResult.Success;

        var otherDisplayName = property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? OtherProperty;
        var message = string.Format(ErrorMessageString, validationContext.DisplayName, otherDisplayName);
        var memberNames = validationContext.MemberName is { } memberName
            ? new[] { memberName } : Array.Empty<string>();
        return new ValidationResult(message, memberNames);
    }
}
