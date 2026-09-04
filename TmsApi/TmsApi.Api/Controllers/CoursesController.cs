using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Api.Filters;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class CoursesController(
    ICourseService courseService,
    LinkGenerator linkGenerator,
    TmsDbContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CourseResponseDto>), StatusCodes.Status200OK)]
    [EndpointSummary("List courses with pagination")]
    [EndpointDescription("Returns a paginated, optionally filtered list of TMS courses. PageSize is capped at 50.")]
    public async Task<IActionResult> GetCourses(
        [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await courseService.GetCoursesAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}", Name = nameof(GetCourseById))]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a course by ID")]
    [EndpointDescription("Returns course details with HATEOAS links. Returns 404 if the course does not exist.")]
    public async Task<IActionResult> GetCourseById(int id, CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);
        if (course is null) return NotFound();

        var selfHref = linkGenerator.GetPathByName(HttpContext, nameof(GetCourseById), new { id })
            ?? $"/api/courses/{id}";
        var enrollmentsHref = linkGenerator.GetPathByName(HttpContext, "ListCourseEnrollments", new { courseId = id })
            ?? $"/api/courses/{id}/enrollments";

        var links = new List<LinkDto>
        {
            new(selfHref, "self", "GET"),
            new(selfHref, "update", "PUT"),
            new(selfHref, "delete", "DELETE"),
            new(enrollmentsHref, "enrollments", "GET")
        };

        if (course.EnrollmentCount < course.MaxCapacity)
        {
            links.Add(new(enrollmentsHref, "enroll", "POST"));
        }

        return Ok(new CourseDetailDto
        {
            Id = course.Id,
            Code = course.Code,
            Title = course.Title,
            MaxCapacity = course.MaxCapacity,
            EnrollmentCount = course.EnrollmentCount,
            Links = links
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(CourseResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Create a new course")]
    [EndpointDescription("Creates a course with a unique code. Returns 409 if the course code already exists.")]
    public async Task<IActionResult> CreateCourse(CreateCourseRequest request, CancellationToken ct)
    {
        if (await courseService.CodeExistsAsync(request.Code, ct))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Course code already exists",
                Detail = $"A course with code '{request.Code}' is already registered.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var result = await courseService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
    }

    [HttpGet("top-enrolled")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [EndpointSummary("Top 5 enrolled courses")]
    public async Task<IActionResult> GetTopEnrolledCourses(CancellationToken cancellationToken = default)
    {
        var topCourses = await context.Enrollments
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
