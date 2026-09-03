using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly TmsDbContext _context;

    public TestController(TmsDbContext context)
    {
        _context = context;
    }

    [HttpGet("active-students")]
    public async Task<IActionResult> GetActiveStudents()
    {
        var count = await _context.Students
            .Where(s => s.IsActive && s.GPA >= 3.0m)
            .CountAsync();

        return Ok(new { count });
    }
    [HttpGet("top-students")]
public async Task<IActionResult> GetTopStudents()
{
    var students = await _context.Students
        .Where(s => s.IsActive)
        .OrderByDescending(s => s.GPA)
        .Select(s => new
        {
            s.Name,
            s.GPA
        })
        .ToListAsync();

    return Ok(students);
}
}