using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class ResultViewModel
{
    public int ResultID { get; set; }

    [Required]
    public int EnrollmentID { get; set; }

    [Required]
    public TimeSpan FinishTime { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Position { get; set; }
}