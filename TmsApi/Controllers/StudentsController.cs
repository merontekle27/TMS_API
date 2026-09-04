using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Models;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly TmsDbContext _context;

    public StudentsController(TmsDbContext context)
    {
        _context = context;
    }

    // TODO 1: Pagination OrderBy, Skip((page - 1) * pageSize), Take(pageSize), ToListAsync with CancellationToken.
    // GET: api/students/paged?page=1&pageSize=20
    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedStudents(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var students = await _context.Students
            .OrderBy(s => s.Name) // Stable sort by name before pagination
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _context.Students.CountAsync(cancellationToken);

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Data = students
        });
    }
}
