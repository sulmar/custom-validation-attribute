using System.ComponentModel.DataAnnotations;

namespace GreaterThan.Tests;

public class ExclusiveBetweenAttributeTests
{
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    [InlineData(0, false)]
    [InlineData(11, false)]
    public void Integers_MustBeStrictlyInsideRange(int value, bool expected)
    {
        var result = Validate(value, 1, 10);
        Assert.Equal(expected, result is null);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public void Dates_MustBeStrictlyInsideRange(int offsetDays, bool expected)
    {
        var from = new DateTime(2026, 9, 23);
        var result = Validate(from.AddDays(offsetDays), from, from.AddDays(2));
        Assert.Equal(expected, result is null);
    }

    [Theory]
    [InlineData(10, false)]
    [InlineData(11, true)]
    [InlineData(99, true)]
    [InlineData(100, false)]
    public void Decimals_MustBeStrictlyInsideRange(int value, bool expected)
    {
        var result = Validate((decimal)value / 10, 1m, 10m);
        Assert.Equal(expected, result is null);
    }

    [Fact]
    public void EqualBounds_RejectEveryValue()
    {
        Assert.NotNull(Validate(5, 5, 5));
        Assert.NotNull(Validate(4, 5, 5));
        Assert.NotNull(Validate(6, 5, 5));
    }

    [Fact]
    public void NullValue_IsAccepted()
    {
        Assert.Null(Validate(null, 1, 10));
    }

    [Theory]
    [InlineData(10, 1)]
    public void FromGreaterThanTo_ThrowsConfigurationError(int from, int to)
    {
        Assert.Throws<InvalidOperationException>(() => new global::GreaterThan.ExclusiveBetweenAttribute(from, to));
    }

    [Fact]
    public void NullBounds_ThrowConfigurationError()
    {
        Assert.Throws<InvalidOperationException>(() => new global::GreaterThan.ExclusiveBetweenAttribute(null!, 10));
        Assert.Throws<InvalidOperationException>(() => new global::GreaterThan.ExclusiveBetweenAttribute(1, null!));
    }

    [Fact]
    public void DifferentBoundTypes_ThrowConfigurationError()
    {
        Assert.Throws<InvalidOperationException>(() => new global::GreaterThan.ExclusiveBetweenAttribute(1, 10m));
    }

    [Fact]
    public void NonComparableBounds_ThrowConfigurationError()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new global::GreaterThan.ExclusiveBetweenAttribute(new object(), new object()));
    }

    [Fact]
    public void DifferentValueType_ThrowsConfigurationError()
    {
        Assert.Throws<InvalidOperationException>(() => Validate(5m, 1, 10));
    }

    [Fact]
    public void Error_UsesBoundsWhenDisplayIsMissing()
    {
        var result = Validate(0, 1, 10);
        Assert.NotNull(result);
        Assert.Equal("Value musi być większe niż 1 i mniejsze niż 10.", result.ErrorMessage);
        Assert.Equal(new[] { "Value" }, result.MemberNames);
    }

    [Fact]
    public void CustomMessage_UsesDisplayNameAndBounds()
    {
        var model = new Product { Id = 0 };
        var context = new ValidationContext(model) { MemberName = nameof(Product.Id) };
        var attribute = new global::GreaterThan.ExclusiveBetweenAttribute(1, 10)
        {
            ErrorMessage = "{0} ({1}, {2})"
        };
        var result = attribute.GetValidationResult(model.Id, context);
        Assert.NotNull(result);
        Assert.Equal("Identyfikator (1, 10)", result.ErrorMessage);
    }

    [Fact]
    public void MissingMemberName_ProducesErrorWithoutMembers()
    {
        var context = new ValidationContext(new object()) { DisplayName = "Wartość" };
        var result = new global::GreaterThan.ExclusiveBetweenAttribute(1, 10).GetValidationResult(0, context);
        Assert.NotNull(result);
        Assert.Empty(result.MemberNames);
        Assert.Equal("Wartość musi być większe niż 1 i mniejsze niż 10.", result.ErrorMessage);
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData(1, false)]
    [InlineData(10, false)]
    public void TryValidateObject_ExecutesAttribute(int id, bool expected)
    {
        var model = new Product { Id = id };
        var errors = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);
        Assert.Equal(expected, valid);
        if (expected)
        {
            Assert.Empty(errors);
        }
        else
        {
            var error = Assert.Single(errors);
            Assert.Equal("Identyfikator musi być większe niż 1 i mniejsze niż 10.", error.ErrorMessage);
            Assert.Equal(new[] { nameof(Product.Id) }, error.MemberNames);
        }
    }

    private static ValidationResult? Validate(object? value, object from, object to)
    {
        var context = new ValidationContext(new object()) { MemberName = "Value", DisplayName = "Value" };
        return new global::GreaterThan.ExclusiveBetweenAttribute(from, to).GetValidationResult(value, context);
    }

    public class Product
    {
        [Display(Name = "Identyfikator")]
        [global::GreaterThan.ExclusiveBetween(1, 10)]
        public int Id { get; set; }
    }
}
