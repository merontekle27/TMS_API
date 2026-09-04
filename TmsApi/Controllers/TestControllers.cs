using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Models;

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

    // Deferred Execution Experiment
    [HttpGet("deferred")]
    public IActionResult TestDeferred()
    {
        Console.WriteLine("\n>>> STEP 1: Building the query object (no database contact)...");
        var query = _context.Students.Where(s => s.GPA >= 3.0m);

        Console.WriteLine(">>> STEP 2: Appending a sorting clause...");
        var orderedQuery = query.OrderBy(s => s.Name);

        Console.WriteLine(">>> STEP 3: Materializing query into a C# List...");
        var results = orderedQuery.ToList(); // Execution is triggered here

        Console.WriteLine(">>> STEP 4: Materialization finished. List populated.\n");
        return Ok(results);
    }

    // Helper for Translation Failure Experiment
    private static bool IsHonorRoll(decimal gpa)
    {
        return gpa >= 3.5m;
    }

    // SQL Translation Failure Experiment
    [HttpGet("translation-fail")]
    public IActionResult TestTranslationFail()
    {
        Console.WriteLine("\n>>> STEP 1: Running non-translatable query...");
        try
        {
            var students = _context.Students
                .Where(s => IsHonorRoll(s.GPA))
                .ToList();
            return Ok(students);
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");
            return BadRequest(new { Message = ex.Message });
        }
    }

    // Registrar Business Query 1: How many active students have GPA >= 3.0?
    [HttpGet("active-students-count")]
    public async Task<IActionResult> GetActiveStudentsCount()
    {
        var count = await _context.Students
            .Where(s => s.IsActive && s.GPA >= 3.0m)
            .CountAsync();

        return Ok(new { count });
    }

    // Registrar Business Query 2: Which courses have the most enrollments, sorted descending?
    [HttpGet("courses-by-enrollments")]
    public async Task<IActionResult> GetCoursesByEnrollments()
    {
        var list = await _context.Courses
            .Select(c => new
            {
                c.Title,
                EnrollmentCount = c.Enrollments.Count
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .ToListAsync();

        return Ok(list);
    }

    // Registrar Business Query 3: What is the average GPA per course?
    [HttpGet("average-gpa-per-course")]
    public async Task<IActionResult> GetAverageGpaPerCourse()
    {
        var list = await _context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                Course = g.Key,
                AverageGPA = g.Average(e => e.Student.GPA)
            })
            .ToListAsync();

        return Ok(list);
    }

    // Registrar Business Query 4 (Approach A - Subquery): Which students have zero enrollments?
    [HttpGet("zero-enrollments-subquery")]
    public async Task<IActionResult> GetStudentsZeroEnrollmentsSubquery()
    {
        var list = await _context.Students
            .Where(s => !s.Enrollments.Any())
            .Select(s => s.Name)
            .ToListAsync();

        return Ok(list);
    }

    // Registrar Business Query 4 (Approach B - EF Core 10 LeftJoin): Which students have zero enrollments?
    [HttpGet("zero-enrollments-leftjoin")]
    public async Task<IActionResult> GetStudentsZeroEnrollmentsLeftJoin()
    {
        var list = await _context.Students
            .LeftJoin(_context.Enrollments,
                s => s.Id,
                e => e.StudentId,
                (s, e) => new { s, e })
            .Where(x => x.e == null)
            .Select(x => x.s.Name)
            .ToListAsync();

        return Ok(list);
    }
}