using System.ComponentModel.DataAnnotations;

public class EndDateAfterStartAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        var reservation = (Reservation)validationContext.ObjectInstance;

        if (value is DateTime endDate && endDate < reservation.StartDate)
        {
            return new ValidationResult(
                "Data końca nie może być wcześniejsza niż data początku.",
                new[] { nameof(Reservation.EndDate) });
        }

        return ValidationResult.Success;
    }
}

