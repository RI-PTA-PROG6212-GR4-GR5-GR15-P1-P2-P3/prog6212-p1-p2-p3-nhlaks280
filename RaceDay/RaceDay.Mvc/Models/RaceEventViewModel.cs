
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models;

public class RaceEventViewModel
{
    public int EventID { get; set; }

    public int OrganiserID { get; set; }

    [Required]
    [StringLength(150)]
    public string EventName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Range(0.1, 1000)]
    public double DistanceKm { get; set; }

    [Required]
    public string EventType { get; set; } = string.Empty;

    public string? BannerUrl { get; set; }

    public IFormFile? BannerImage { get; set; }
}
