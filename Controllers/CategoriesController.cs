using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    public CategoriesController(AppDbContext context) { _context = context; }

    [HttpGet("/api/events/{eventId}/categories")]
    public async Task<IActionResult> GetCategoriesForEvent(int eventId)
    {
        var categories = await _context.Categories.Where(c => c.EventID == eventId).ToListAsync();
        return Ok(categories);
    }

    [HttpPost("/api/events/{eventId}/categories")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> AddCategory(int eventId, [FromBody] Category category)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserID.ToString() != userId) return Forbid();
        category.EventID = eventId;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Category added successfully", categoryId = category.CategoryID });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updated)
    {
        var cat = await _context.Categories.FindAsync(id);
        if (cat == null) return NotFound();
        cat.CategoryName = updated.CategoryName;
        cat.Description = updated.Description;
        await _context.SaveChangesAsync();
        return Ok(new { message = "Category updated successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var cat = await _context.Categories.FindAsync(id);
        if (cat == null) return NotFound();
        _context.Categories.Remove(cat);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Category deleted successfully" });
    }
}