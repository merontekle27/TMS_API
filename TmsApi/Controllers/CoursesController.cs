using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly TmsDbContext _context;

    public CoursesController(TmsDbContext context)
    {
        _context = context;
    }

    // TODO 2: Top 5 courses by enrollment GroupBy, order by count, Take(5).
    // GET: api/courses/top-enrolled
    [HttpGet("top-enrolled")]
    public async Task<IActionResult> GetTopEnrolledCourses(CancellationToken cancellationToken = default)
    {
        var topCourses = await _context.Enrollments
            .GroupBy(e => new { e.CourseId, e.Course.Title })
            .Select(g => new
            {
                CourseId = g.Key.CourseId,
                Title = g.Key.Title,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(c => c.EnrollmentCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        return Ok(topCourses);
    }
}
