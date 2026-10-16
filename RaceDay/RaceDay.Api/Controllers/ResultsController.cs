
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResultsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public ResultsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Results
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Result>>> GetResults()
    {
        var query = _context.Results
            .AsNoTracking()
            .Include(r => r.Enrollment)
            .AsQueryable();

        if (User.IsInRole("Participant"))
        {
            if (!TryGetCurrentUserId(out var participantId))
                return Unauthorized();

            query = query.Where(
                r => r.Enrollment.ParticipantID == participantId);
        }
        else if (!User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        return Ok(await query.ToListAsync());
    }

    // GET: api/Results/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Result>> GetResult(int id)
    {
        var result = await _context.Results
            .AsNoTracking()
            .Include(r => r.Enrollment)
            .FirstOrDefaultAsync(r => r.ResultID == id);

        if (result == null)
            return NotFound("Result not found.");

        if (User.IsInRole("Participant"))
        {
            if (!TryGetCurrentUserId(out var participantId))
                return Unauthorized();

            if (result.Enrollment.ParticipantID != participantId)
                return Forbid();
        }
        else if (!User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        return Ok(result);
    }

    // POST: api/Results
    // Only organisers can record results.
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<ActionResult<Result>> CreateResult(
        [FromBody] Result result)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (result.ResultID != 0)
            return BadRequest(
                "Do not provide a ResultID when creating a result.");

        if (result.FinishTime <= TimeSpan.Zero)
            return BadRequest("Finish time must be greater than zero.");

        if (result.Position <= 0)
            return BadRequest("Position must be greater than zero.");

        var enrollmentExists = await _context.Enrollments.AnyAsync(
            e => e.EnrollmentID == result.EnrollmentID);

        if (!enrollmentExists)
            return BadRequest("The specified enrolment does not exist.");

        var resultExists = await _context.Results.AnyAsync(
            r => r.EnrollmentID == result.EnrollmentID);

        if (resultExists)
            return Conflict(
                "A result already exists for this enrolment.");

        _context.Results.Add(result);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetResult),
            new { id = result.ResultID },
            result);
    }

    // PUT: api/Results/5
    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateResult(
        int id,
        [FromBody] Result updatedResult)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (updatedResult.ResultID != 0 &&
            updatedResult.ResultID != id)
            return BadRequest(
                "The route ID must match the result ID.");

        if (updatedResult.FinishTime <= TimeSpan.Zero)
            return BadRequest("Finish time must be greater than zero.");

        if (updatedResult.Position <= 0)
            return BadRequest("Position must be greater than zero.");

        var result = await _context.Results
            .FirstOrDefaultAsync(r => r.ResultID == id);

        if (result == null)
            return NotFound("Result not found.");

        // Keep the original enrolment; this endpoint updates result details only.
        result.FinishTime = updatedResult.FinishTime;
        result.Position = updatedResult.Position;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out userId);
    }
}

