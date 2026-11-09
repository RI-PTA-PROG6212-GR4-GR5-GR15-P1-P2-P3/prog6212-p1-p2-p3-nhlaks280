
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventCategoriesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EventCategoriesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/EventCategories
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventCategory>>> GetEventCategories()
    {
        var eventCategories = await _context.EventCategories
            .AsNoTracking()
            .Include(ec => ec.Event)
            .Include(ec => ec.Category)
            .ToListAsync();

        return Ok(eventCategories);
    }

    // GET: api/EventCategories/5
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventCategory>> GetEventCategory(int id)
    {
        var eventCategory = await _context.EventCategories
            .AsNoTracking()
            .Include(ec => ec.Event)
            .Include(ec => ec.Category)
            .FirstOrDefaultAsync(ec => ec.EventCategoryID == id);

        if (eventCategory == null)
            return NotFound("Event category not found.");

        return Ok(eventCategory);
    }

    // POST: api/EventCategories
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<ActionResult<EventCategory>> CreateEventCategory(
        [FromBody] EventCategory eventCategory)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (eventCategory.EventCategoryID != 0)
            return BadRequest("Do not provide an EventCategoryID.");

        var eventExists = await _context.Events
            .AnyAsync(e => e.EventID == eventCategory.EventID);

        var categoryExists = await _context.Categories
            .AnyAsync(c => c.CategoryID == eventCategory.CategoryID);

        if (!eventExists || !categoryExists)
            return BadRequest("A valid event and category are required.");

        var duplicate = await _context.EventCategories.AnyAsync(ec =>
            ec.EventID == eventCategory.EventID &&
            ec.CategoryID == eventCategory.CategoryID);

        if (duplicate)
            return Conflict("This category is already assigned to the event.");

        _context.EventCategories.Add(eventCategory);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEventCategory),
            new { id = eventCategory.EventCategoryID },
            eventCategory);
    }
}
