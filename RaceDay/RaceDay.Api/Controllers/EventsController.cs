
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EventsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Events
    // Anyone can view events.
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RaceEvent>>> GetEvents()
    {
        var events = await _context.Events
            .AsNoTracking()
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/Events/5
    // Anyone can view an individual event.
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RaceEvent>> GetEvent(int id)
    {
        var raceEvent = await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EventID == id);

        if (raceEvent == null)
        {
            return NotFound("Event not found.");
        }

        return Ok(raceEvent);
    }

    // POST: api/Events
    // Only organisers can create events.
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<ActionResult<RaceEvent>> CreateEvent(
        [FromBody] RaceEvent raceEvent)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (raceEvent.EventID != 0)
        {
            return BadRequest(
                "Do not provide an EventID when creating an event.");
        }

        if (raceEvent.DistanceKm <= 0)
        {
            return BadRequest(
                "Event distance must be greater than zero.");
        }

        if (!IsValidEventType(raceEvent.EventType))
        {
            return BadRequest(
                "Event type must be Run, Walk, or Cycle.");
        }

        var organiser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.UserID == raceEvent.OrganiserID);

        if (organiser == null || organiser.Role != "Organiser")
        {
            return BadRequest(
                "A valid organiser account is required.");
        }

        raceEvent.EventType = NormalizeEventType(raceEvent.EventType);

        _context.Events.Add(raceEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvent),
            new { id = raceEvent.EventID },
            raceEvent);
    }

    // PUT: api/Events/5
    // Only organisers can update events.
    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEvent(
        int id,
        [FromBody] RaceEvent updatedEvent)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (updatedEvent.EventID != 0 &&
            updatedEvent.EventID != id)
        {
            return BadRequest(
                "The route ID must match the event ID.");
        }

        if (updatedEvent.DistanceKm <= 0)
        {
            return BadRequest(
                "Event distance must be greater than zero.");
        }

        if (!IsValidEventType(updatedEvent.EventType))
        {
            return BadRequest(
                "Event type must be Run, Walk, or Cycle.");
        }

        var existingEvent = await _context.Events
            .FirstOrDefaultAsync(e => e.EventID == id);

        if (existingEvent == null)
        {
            return NotFound("Event not found.");
        }

        var organiser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.UserID == updatedEvent.OrganiserID);

        if (organiser == null || organiser.Role != "Organiser")
        {
            return BadRequest(
                "A valid organiser account is required.");
        }

        existingEvent.OrganiserID = updatedEvent.OrganiserID;
        existingEvent.EventName = updatedEvent.EventName;
        existingEvent.Description = updatedEvent.Description;
        existingEvent.EventDate = updatedEvent.EventDate;
        existingEvent.Location = updatedEvent.Location;
        existingEvent.DistanceKm = updatedEvent.DistanceKm;
        existingEvent.EventType =
            NormalizeEventType(updatedEvent.EventType);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Events/5
    // Only organisers can delete events.
    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var raceEvent = await _context.Events
            .FirstOrDefaultAsync(e => e.EventID == id);

        if (raceEvent == null)
        {
            return NotFound("Event not found.");
        }

        _context.Events.Remove(raceEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static bool IsValidEventType(string? eventType)
    {
        return !string.IsNullOrWhiteSpace(eventType) &&
            (eventType.Equals(
                "Run", StringComparison.OrdinalIgnoreCase) ||
             eventType.Equals(
                "Walk", StringComparison.OrdinalIgnoreCase) ||
             eventType.Equals(
                "Cycle", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeEventType(string eventType)
    {
        if (eventType.Equals(
            "Run", StringComparison.OrdinalIgnoreCase))
            return "Run";

        if (eventType.Equals(
            "Walk", StringComparison.OrdinalIgnoreCase))
            return "Walk";

        return "Cycle";
    }
}
