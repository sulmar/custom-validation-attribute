namespace GreaterThan;

[AttributeUsage(AttributeTargets.Property)]
public sealed class LessThanAttribute(string otherProperty)
    : CompareAttribute(otherProperty, "{0} musi być mniejsze niż {1}.")
{
    protected override bool Predicate(int result) =>  result < 0;
    
}