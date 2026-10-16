
namespace RaceDay.Api.Models;

public class EventCategory
{
    public int EventCategoryID { get; set; }
    public int EventID { get; set; }
    public int CategoryID { get; set; }

    public RaceEvent? Event { get; set; }
    public Category? Category { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
