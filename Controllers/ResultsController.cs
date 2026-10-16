using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ResultsController(AppDbContext context) { _context = context; }

    [HttpPost("/api/enrolments/{enrolmentId}/result")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> AddResult(int enrolmentId, [FromBody] Result newResult)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var enrolment = await _context.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.EnrolmentID == enrolmentId);
        if (enrolment == null) return NotFound();
        if (enrolment.Event != null && enrolment.Event.OrganiserID.ToString() != userId) return Forbid();

        if (await _context.Results.AnyAsync(r => r.EnrolmentID == enrolmentId))
            return BadRequest(new { message = "Result already exists" });

        newResult.EnrolmentID = enrolmentId;
        _context.Results.Add(newResult);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Result added successfully", resultId = newResult.ResultID });
    }

    [HttpGet("/api/events/{eventId}/results")]
    public async Task<IActionResult> GetResultsForEvent(int eventId)
    {
        var results = await _context.Results
            .Include(r => r.Enrolment).ThenInclude(e => e!.Participant)
            .Where(r => r.Enrolment != null && r.Enrolment.EventID == eventId)
            .Select(r => new { r.ResultID, ParticipantName = r.Enrolment!.Participant!.FullName, r.FinishTime, r.Position, r.PointsEarned, r.IsDisqualified })
            .OrderBy(r => r.Position)
            .ToListAsync();
        return Ok(results);
    }

    [HttpGet("/api/users/{userId}/results")]
    [Authorize]
    public async Task<IActionResult> GetResultsForUser(int userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userIdClaim != userId.ToString() && role != "Admin") return Forbid();

        var results = await _context.Results
            .Include(r => r.Enrolment).ThenInclude(e => e!.Event)
            .Where(r => r.Enrolment != null && r.Enrolment.ParticipantID == userId)
            .Select(r => new { r.ResultID, EventName = r.Enrolment!.Event!.EventName, r.FinishTime, r.Position, r.PointsEarned })
            .ToListAsync();
        return Ok(results);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> UpdateResult(int id, [FromBody] Result updated)
    {
        var result = await _context.Results.FindAsync(id);
        if (result == null) return NotFound();
        result.FinishTime = updated.FinishTime;
        result.Position = updated.Position;
        result.PointsEarned = updated.PointsEarned;
        result.IsDisqualified = updated.IsDisqualified;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Result updated successfully" });
    }
}