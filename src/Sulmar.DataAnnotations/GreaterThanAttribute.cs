namespace Sulmar.DataAnnotations;

// Wzorzec projektowy Template Method

[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanAttribute(string otherProperty)
    : CompareAttribute(otherProperty, "{0} musi być większe niż {1}.")
{
    protected override bool Predicate(int result) => result > 0;
}