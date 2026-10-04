namespace Sulmar.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class GreaterThanOrEqualToAttribute(string otherProperty)
    : CompareAttribute(otherProperty, "{0} musi być większe lub równe {1}.")
{
    override protected bool Predicate(int result) =>  result >= 0;
}