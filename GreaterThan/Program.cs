using System.ComponentModel.DataAnnotations;

var reservation = new Reservation
{
    StartDate = new DateTime(2026, 9, 23),
    EndDate = new DateTime(2026, 9, 22), // Celowo wcześniejsza data
    Surname = "Foo"
};

var results = new List<ValidationResult>();

var isValid = Validator.TryValidateObject(
    reservation,
    new ValidationContext(reservation),
    results,
    validateAllProperties: true);

Console.WriteLine($"Poprawny: {isValid}");

foreach (var result in results)
{
    Console.WriteLine(
        $"{string.Join(", ", result.MemberNames)}: {result.ErrorMessage}");
}