using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class ParticipantEventCategoryViewModel
{
    public int EventCategoryID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }

    public ParticipantEventViewModel? Event { get; set; }
    public ParticipantCategoryViewModel? Category { get; set; }
}

public class ParticipantEventViewModel
{
    public int EventID { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? Location { get; set; }
    public double DistanceKm { get; set; }
    public string? EventType { get; set; }
}

public class ParticipantCategoryViewModel
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateEnrollmentViewModel
{
    [Required]
    public int EventCategoryID { get; set; }
}

public class ParticipantEnrollmentViewModel
{
    public int EnrollmentID { get; set; }
    public int ParticipantID { get; set; }
    public int EventCategoryID { get; set; }
    public DateTime EnrollmentDate { get; set; }
}