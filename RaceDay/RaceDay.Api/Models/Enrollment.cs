
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class Enrollment
{
    public int EnrollmentID { get; set; }
    public int ParticipantID { get; set; }
    public int EventCategoryID { get; set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public AppUser? Participant { get; set; }
    public EventCategory? EventCategory { get; set; }
    public Result? Result { get; set; }
}
