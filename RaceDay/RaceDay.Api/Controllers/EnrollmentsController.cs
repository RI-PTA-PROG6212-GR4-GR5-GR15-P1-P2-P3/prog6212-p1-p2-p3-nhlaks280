
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
public class EnrollmentsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EnrollmentsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Enrollments
    // Organisers can view all enrolments; participants can view their own.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
    {
        var query = _context.Enrollments
            .AsNoTracking()
            .AsQueryable();

        if (User.IsInRole("Participant"))
        {
            if (!TryGetCurrentUserId(out var participantId))
                return Unauthorized();

            query = query.Where(e => e.ParticipantID == participantId);
        }
        else if (!User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        return Ok(await query.ToListAsync());
    }

    // GET: api/Enrollments/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
    {
        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EnrollmentID == id);

        if (enrollment == null)
            return NotFound("Enrolment not found.");

        if (User.IsInRole("Participant"))
        {
            if (!TryGetCurrentUserId(out var participantId))
                return Unauthorized();

            if (enrollment.ParticipantID != participantId)
                return Forbid();
        }
        else if (!User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        return Ok(enrollment);
    }

    // POST: api/Enrollments
    // Participants can enrol only themselves.
    [Authorize(Roles = "Participant")]
    [HttpPost]
    public async Task<ActionResult<Enrollment>> CreateEnrollment(
        [FromBody] Enrollment enrollment)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (!TryGetCurrentUserId(out var participantId))
            return Unauthorized();

        // Never trust the participant ID supplied by the client.
        enrollment.ParticipantID = participantId;

        if (enrollment.EnrollmentID != 0)
            return BadRequest(
                "Do not provide an EnrollmentID when creating an enrolment.");

        var participantExists = await _context.Users.AnyAsync(
            u => u.UserID == participantId && u.Role == "Participant");

        if (!participantExists)
            return BadRequest("A valid participant account is required.");

        var eventCategory = await _context.EventCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                ec => ec.EventCategoryID == enrollment.EventCategoryID);

        if (eventCategory == null)
            return BadRequest("The selected event category does not exist.");

        var alreadyEnrolled = await _context.Enrollments.AnyAsync(
            e => e.ParticipantID == participantId &&
                 e.EventCategoryID == enrollment.EventCategoryID);

        if (alreadyEnrolled)
            return Conflict("You are already enrolled in this event category.");

        enrollment.EnrollmentDate = DateTime.UtcNow;

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEnrollment),
            new { id = enrollment.EnrollmentID },
            enrollment);
    }

    // DELETE: api/Enrollments/5
    // Participants can cancel their own enrolment.
    // Organisers can manage enrolments.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.EnrollmentID == id);

        if (enrollment == null)
            return NotFound("Enrolment not found.");

        if (User.IsInRole("Participant"))
        {
            if (!TryGetCurrentUserId(out var participantId))
                return Unauthorized();

            if (enrollment.ParticipantID != participantId)
                return Forbid();
        }
        else if (!User.IsInRole("Organiser"))
        {
            return Forbid();
        }

        var hasResult = await _context.Results.AnyAsync(
            r => r.EnrollmentID == id);

        if (hasResult)
            return Conflict(
                "This enrolment has a result and cannot be deleted.");

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        return int.TryParse(claim?.Value, out userId);
    }
}

