using System.ComponentModel.DataAnnotations;
using Sulmar.DataAnnotations;

namespace Sulmar.DataAnnotations.Tests;

public class MustAttributeTests
{
    [Fact]
    public void FooSurname_WithLaterEndDate_IsValid()
    {
        var result = Validate(new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 24),
            Surname = "Foo"
        });

        Assert.Null(result);
    }

    [Fact]
    public void OtherSurname_IsInvalid()
    {
        var result = Validate(new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 24),
            Surname = "Bar"
        });

        Assert.NotNull(result);
        Assert.Equal("Nazwisko nie spełnia warunku.", result.ErrorMessage);
        Assert.Equal(new[] { nameof(Reservation.Surname) }, result.MemberNames);
    }

    [Fact]
    public void FooSurname_WithEarlierEndDate_IsInvalid()
    {
        var result = Validate(new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 22),
            Surname = "Foo"
        });

        Assert.NotNull(result);
        Assert.Equal("Nazwisko nie spełnia warunku.", result.ErrorMessage);
    }

    [Fact]
    public void NullSurname_IsAccepted()
    {
        var result = Validate(new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 22),
            Surname = null
        });

        Assert.Null(result);
    }

    [Fact]
    public void MissingMemberName_ProducesErrorWithoutMembers()
    {
        var reservation = new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 24),
            Surname = "Bar"
        };
        var context = new ValidationContext(reservation) { DisplayName = "Nazwisko" };
        var result = new MustAttribute("IsFoo").GetValidationResult(reservation.Surname, context);

        Assert.NotNull(result);
        Assert.Equal("Nazwisko nie spełnia warunku.", result.ErrorMessage);
        Assert.Empty(result.MemberNames);
    }

    [Fact]
    public void MissingMethod_ThrowsConfigurationError()
    {
        var model = new MissingMethodModel();
        var context = new ValidationContext(model) { MemberName = nameof(MissingMethodModel.Surname) };

        Assert.Throws<InvalidOperationException>(() =>
            new MustAttribute("DoesNotExist").GetValidationResult(model.Surname, context));
    }

    [Fact]
    public void WrongSignature_ThrowsConfigurationError()
    {
        var model = new WrongSignatureModel();
        var context = new ValidationContext(model) { MemberName = nameof(WrongSignatureModel.Surname) };
        var attribute = new MustAttribute(nameof(WrongSignatureModel.NotObjectAndValue));

        Assert.Throws<InvalidOperationException>(() =>
            attribute.GetValidationResult(model.Surname, context));
    }

    [Theory]
    [InlineData("Foo", 1, true)]
    [InlineData("Foo", -1, false)]
    [InlineData("Bar", 1, false)]
    public void TryValidateObject_ExecutesMust(string surname, int offsetDays, bool expected)
    {
        var model = new Reservation
        {
            StartDate = new DateTime(2026, 9, 23),
            EndDate = new DateTime(2026, 9, 23).AddDays(offsetDays),
            Surname = surname
        };
        var errors = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);

        Assert.Equal(expected, valid);
        if (expected)
        {
            Assert.Empty(errors);
            return;
        }

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(Reservation.Surname)));
    }

    private static ValidationResult? Validate(Reservation reservation)
    {
        var context = new ValidationContext(reservation) { MemberName = nameof(Reservation.Surname) };
        return new MustAttribute("IsFoo").GetValidationResult(reservation.Surname, context);
    }

    private sealed class MissingMethodModel
    {
        public string Surname { get; set; } = "Foo";
    }

    private sealed class WrongSignatureModel
    {
        public string Surname { get; set; } = "Foo";

        public static bool NotObjectAndValue(string surname) => surname == "Foo";
    }
}
