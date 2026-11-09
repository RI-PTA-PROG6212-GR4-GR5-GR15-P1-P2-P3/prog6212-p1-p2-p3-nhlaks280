using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class EventCategoryViewModel
{
    public int EventCategoryID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }
    public RaceEventViewModel? Event { get; set; }
    public CategoryViewModel? Category { get; set; }
}

public class CategoryViewModel
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = "";
    public string? Description { get; set; }
}

public class EnrollmentCreateViewModel
{
    [Required]
    public int EventCategoryID { get; set; }
}

public class EnrollmentResponseViewModel
{
    public int EnrollmentID { get; set; }
    public int ParticipantID { get; set; }
    public int EventCategoryID { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public EventCategoryViewModel? EventCategory { get; set; }
}