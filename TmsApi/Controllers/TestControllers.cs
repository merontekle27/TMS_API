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
}