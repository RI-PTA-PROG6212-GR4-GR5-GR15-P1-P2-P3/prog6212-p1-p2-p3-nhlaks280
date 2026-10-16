
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Result
{
    public int ResultID { get; set; }
    public int EnrollmentID { get; set; }

    public TimeSpan FinishTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Position { get; set; }

    public Enrollment? Enrollment { get; set; }
}
