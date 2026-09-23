using System.ComponentModel.DataAnnotations;
using GreaterThan;

public class Reservation
{
    [Display(Name = "Data początku")]
    public DateTime StartDate { get; set; }

    [EndDateAfterStart]
    [Display(Name = "Data końca")]
    [GreaterThan(nameof(StartDate))]
    public DateTime EndDate { get; set; }
}