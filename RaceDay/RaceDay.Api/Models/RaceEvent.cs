
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class RaceEvent
{
    public int EventID { get; set; }
    public int OrganiserID { get; set; }

    [Required, MaxLength(100)]
    public string EventName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    [Required, MaxLength(20)]
    public string EventType { get; set; } = "Run";

    public AppUser? Organiser { get; set; }
    public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
}
