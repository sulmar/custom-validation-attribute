namespace Sulmar.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class InclusiveBetweenAttribute(object from, object to)
    : BetweenAttribute(from, to, "{0} musi być większe lub równe {1} i mniejsze lub równe {2}.")
{
    protected override bool Predicate(int fromComparison, int toComparison) =>
        fromComparison >= 0 && toComparison <= 0;
}
