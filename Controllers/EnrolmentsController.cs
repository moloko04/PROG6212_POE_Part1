using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly AppDbContext _context;
    public EnrolmentsController(AppDbContext context) { _context = context; }

    [HttpPost("/api/events/{eventId}/enrol")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> EnrolInEvent(int eventId, [FromBody] Enrolment enrolment)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();
        var participantId = int.Parse(userId);

        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound();

        if (await _context.Enrolments.AnyAsync(e => e.EventID == eventId && e.ParticipantID == participantId))
            return BadRequest(new { message = "Already enrolled" });

        var newEnrolment = new Enrolment
        {
            EventID = eventId,
            ParticipantID = participantId,
            EnrolmentDate = DateTime.Now,
            Status = "Confirmed"
        };

        _context.Enrolments.Add(newEnrolment);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Enrolment successful", enrolmentId = newEnrolment.EnrolmentID });
    }

    [HttpGet("/api/events/{eventId}/enrolments")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetEnrolmentsForEvent(int eventId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserID.ToString() != userId) return Forbid();

        var enrolments = await _context.Enrolments
            .Where(e => e.EventID == eventId)
            .Include(e => e.Participant)
            .Select(e => new { e.EnrolmentID, e.ParticipantID, ParticipantName = e.Participant!.FullName, e.EnrolmentDate, e.Status })
            .ToListAsync();
        return Ok(enrolments);
    }

    [HttpGet("/api/users/{userId}/enrolments")]
    [Authorize]
    public async Task<IActionResult> GetEnrolmentsForUser(int userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userIdClaim != userId.ToString() && role != "Admin") return Forbid();

        var enrolments = await _context.Enrolments
            .Where(e => e.ParticipantID == userId)
            .Include(e => e.Event)
            .Select(e => new { e.EnrolmentID, e.EventID, EventName = e.Event!.EventName, e.Event.EventDate, e.Status })
            .ToListAsync();
        return Ok(enrolments);
    }
}