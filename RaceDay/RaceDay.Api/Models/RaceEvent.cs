
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class RaceEvent
{
    public int EventID { get; set; }

    public int OrganiserID { get; set; }

    [Required]
    [MaxLength(150)]
    public string EventName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public double DistanceKm { get; set; }

    [Required]
    [MaxLength(50)]
    public string EventType { get; set; } = string.Empty;

    public string? BannerUrl { get; set; }

    public AppUser? Organiser { get; set; }

    public ICollection<EventCategory> EventCategories { get; set; }
        = new List<EventCategory>();
}
