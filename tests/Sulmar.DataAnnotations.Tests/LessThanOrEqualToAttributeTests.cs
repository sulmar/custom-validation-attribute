using System.ComponentModel.DataAnnotations;

namespace Sulmar.DataAnnotations.Tests;

public class LessThanOrEqualToAttributeTests
{
    [Theory]
    [InlineData(9, 10, true)]
    [InlineData(10, 10, true)]
    [InlineData(11, 10, false)]
    [InlineData(-2, -1, true)]
    [InlineData(-1, -1, true)]
    public void Integers_MustBeLessOrEqual(int value, int other, bool expected)
    {
        var result = Validate(value, new Values { Other = other });
        Assert.Equal(expected, result is null);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void Dates_MustBeLessOrEqual(int offsetDays, bool expected)
    {
        var start = new DateTime(2026, 9, 23);
        var result = Validate(start.AddDays(offsetDays), new Values { Other = start });
        Assert.Equal(expected, result is null);
    }

    [Theory]
    [InlineData(9, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public void Decimals_MustBeLessOrEqual(int value, bool expected)
    {
        var result = Validate((decimal)value / 10, new Values { Other = 1m });
        Assert.Equal(expected, result is null);
    }

    [Theory]
    [InlineData(null, 10)]
    [InlineData(10, null)]
    [InlineData(null, null)]
    public void NullValues_AreAccepted(int? value, int? other)
    {
        Assert.Null(Validate(value, new Values { Other = other }));
    }

    [Theory]
    [InlineData("Missing")]
    [InlineData("WriteOnly")]
    [InlineData("Item")]
    [InlineData("StaticValue")]
    [InlineData("PrivateValue")]
    public void InvalidProperty_ThrowsConfigurationError(string propertyName)
    {
        Assert.Throws<InvalidOperationException>(() =>
            Validate(10, new InvalidProperties(), propertyName));
    }

    [Fact]
    public void DifferentTypes_ThrowConfigurationError()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Validate(9, new Values { Other = 10m }));
    }

    [Fact]
    public void NonComparableValues_ThrowConfigurationError()
    {
        Assert.Throws<InvalidOperationException>(() =>
            Validate(new object(), new Values { Other = new object() }));
    }

    [Fact]
    public void Error_UsesPropertyNameWhenDisplayIsMissing()
    {
        var result = Validate(2, new Values { Other = 1 });
        Assert.NotNull(result);
        Assert.Equal("Value musi być mniejsze lub równe Other.", result.ErrorMessage);
        Assert.Equal(new[] { "Value" }, result.MemberNames);
    }

    [Fact]
    public void CustomMessage_UsesDisplayNames()
    {
        var model = new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 24)
        };
        var context = new ValidationContext(model) { MemberName = nameof(Reservation.EndDate) };
        var attribute = new Sulmar.DataAnnotations.LessThanOrEqualToAttribute(nameof(Reservation.StartDate))
        {
            ErrorMessage = "{0} <= {1}"
        };
        var result = attribute.GetValidationResult(model.EndDate, context);
        Assert.NotNull(result);
        Assert.Equal("Data końca <= Data początku", result.ErrorMessage);
    }

    [Fact]
    public void MissingMemberName_ProducesErrorWithoutMembers()
    {
        var context = new ValidationContext(new Values { Other = 1 }) { DisplayName = "Wartość" };
        var result = new Sulmar.DataAnnotations.LessThanOrEqualToAttribute("Other").GetValidationResult(2, context);
        Assert.NotNull(result);
        Assert.Empty(result.MemberNames);
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, false)]
    public void TryValidateObject_ExecutesAttribute(int offsetDays, bool expected)
    {
        var model = new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 23).AddDays(offsetDays)
        };
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
            Assert.Equal("Data końca musi być mniejsze lub równe Data początku.", error.ErrorMessage);
            Assert.Equal(new[] { nameof(Reservation.EndDate) }, error.MemberNames);
        }
    }

    private static ValidationResult? Validate(object? value, object model, string otherProperty = "Other")
    {
        var context = new ValidationContext(model) { MemberName = "Value", DisplayName = "Value" };
        return new Sulmar.DataAnnotations.LessThanOrEqualToAttribute(otherProperty).GetValidationResult(value, context);
    }

    public class Values
    {
        public object? Other { get; set; }
    }

    public class InvalidProperties
    {
        public int WriteOnly { set { } }
        public int this[int index] => index;
        public static int StaticValue => 0;
        private int PrivateValue => 0;
    }

    public class Reservation
    {
        [Display(Name = "Data początku")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data końca")]
        [Sulmar.DataAnnotations.LessThanOrEqualTo(nameof(StartDate))]
        public DateTime EndDate { get; set; }
    }
}
