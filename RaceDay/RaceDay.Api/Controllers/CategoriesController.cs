
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public CategoriesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Categories
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/Categories/5
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
            return NotFound("Category not found.");

        return Ok(category);
    }

    // POST: api/Categories
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(
        [FromBody] Category category)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (category.CategoryID != 0)
            return BadRequest("Do not provide a CategoryID when creating a category.");

        if (string.IsNullOrWhiteSpace(category.CategoryName))
            return BadRequest("Category name is required.");

        var name = category.CategoryName.Trim();

        if (await _context.Categories.AnyAsync(
            c => c.CategoryName == name))
            return Conflict("A category with this name already exists.");

        category.CategoryName = name;
        category.Description = category.Description?.Trim();

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.CategoryID },
            category);
    }

    // PUT: api/Categories/5
    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        [FromBody] Category updatedCategory)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (updatedCategory.CategoryID != 0 &&
            updatedCategory.CategoryID != id)
            return BadRequest("The route ID must match the category ID.");

        if (string.IsNullOrWhiteSpace(updatedCategory.CategoryName))
            return BadRequest("Category name is required.");

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
            return NotFound("Category not found.");

        var name = updatedCategory.CategoryName.Trim();

        if (await _context.Categories.AnyAsync(
            c => c.CategoryID != id && c.CategoryName == name))
            return Conflict("A category with this name already exists.");

        category.CategoryName = name;
        category.Description = updatedCategory.Description?.Trim();

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Categories/5
    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryID == id);

        if (category == null)
            return NotFound("Category not found.");

        var isInUse = await _context.EventCategories
            .AnyAsync(ec => ec.CategoryID == id);

        if (isInUse)
            return Conflict(
                "This category is assigned to an event and cannot be deleted.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
