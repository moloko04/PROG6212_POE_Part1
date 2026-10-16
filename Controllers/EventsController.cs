using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _context;
    public EventsController(AppDbContext context) { _context = context; }

    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _context.Events.Select(e => new { e.EventID, e.EventName, e.EventDate, e.Location, e.Description, e.Status, e.OrganiserID }).ToListAsync();
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var ev = await _context.Events.Include(e => e.Categories).Include(e => e.Prizes).FirstOrDefaultAsync(e => e.EventID == id);
        if (ev == null) return NotFound(new { message = "Event not found" });
        return Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();
        newEvent.OrganiserID = int.Parse(userId);
        newEvent.CreatedDate = DateTime.Now;
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Event created successfully", eventId = newEvent.EventID });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updated)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserID.ToString() != userId) return Forbid();
        ev.EventName = updated.EventName;
        ev.EventDate = updated.EventDate;
        ev.Location = updated.Location;
        ev.Description = updated.Description;
        ev.Status = updated.Status;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Event updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ev = await _context.Events.FindAsync(id);
        if (ev == null) return NotFound();
        if (ev.OrganiserID.ToString() != userId) return Forbid();
        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Event deleted successfully" });
    }
}