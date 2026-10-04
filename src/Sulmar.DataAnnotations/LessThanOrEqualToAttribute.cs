namespace Sulmar.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class LessThanOrEqualToAttribute(string otherProperty)
    : CompareAttribute(otherProperty, "{0} musi być mniejsze lub równe {1}.")
{
    override protected bool Predicate(int result) =>  result <= 0;
}