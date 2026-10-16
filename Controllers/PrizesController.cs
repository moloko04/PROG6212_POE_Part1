using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrizesController : ControllerBase
{
    private readonly AppDbContext _context;
    public PrizesController(AppDbContext context) { _context = context; }

    [HttpGet("/api/events/{eventId}/prizes")]
    public async Task<IActionResult> GetPrizesForEvent(int eventId)
    {
        var prizes = await _context.Prizes.Include(p => p.Category)
            .Where(p => p.EventID == eventId)
            .Select(p => new { p.PrizeID, p.PrizeName, p.PrizeValue, p.PositionAwarded, CategoryName = p.Category!.CategoryName })
            .ToListAsync();
        return Ok(prizes);
    }

    [HttpPost("/api/events/{eventId}/prizes")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> AddPrize(int eventId, [FromBody] Prize prize)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserID.ToString() != userId) return Forbid();

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == prize.CategoryID && c.EventID == eventId);
        if (category == null) return BadRequest(new { message = "Category not found for this event" });

        prize.EventID = eventId;
        _context.Prizes.Add(prize);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Prize added successfully", prizeId = prize.PrizeID });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> UpdatePrize(int id, [FromBody] Prize updated)
    {
        var prize = await _context.Prizes.FindAsync(id);
        if (prize == null) return NotFound();
        prize.PrizeName = updated.PrizeName;
        prize.PrizeValue = updated.PrizeValue;
        prize.PositionAwarded = updated.PositionAwarded;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Prize updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> DeletePrize(int id)
    {
        var prize = await _context.Prizes.FindAsync(id);
        if (prize == null) return NotFound();
        _context.Prizes.Remove(prize);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Prize deleted successfully" });
    }
}