namespace GreaterThan;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ExclusiveBetweenAttribute(object from, object to)
    : BetweenAttribute(from, to, "{0} musi być większe niż {1} i mniejsze niż {2}.")
{
    protected override bool Predicate(int fromComparison, int toComparison) =>
        fromComparison > 0 && toComparison < 0;
}
