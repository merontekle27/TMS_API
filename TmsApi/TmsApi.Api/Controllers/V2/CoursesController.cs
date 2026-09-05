using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;

namespace TmsApi.Api.Controllers.V2;

public record UpdateCourseRequest(string Title);

[ApiController]
[Route("api/v{version:apiVersion}/courses")]
[Route("api/courses")]
[ApiVersion("2.0")]
public class CoursesController(
    ICachedCourseService cachedCourseService,
    ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var allCourses = await cachedCourseService.GetAllCoursesAsync(ct);
        var totalCount = allCourses.Count;

        var rows = allCourses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var hasNext = page < totalPages;
        var hasPrevious = page > 1;

        return Ok(new
        {
            data = rows,
            meta = new
            {
                totalCount,
                page,
                pageSize,
                totalPages,
                hasNext,
                hasPrevious
            },
            links = new
            {
                self = $"/api/v2/courses?page={page}&pageSize={pageSize}",
                next = hasNext
                    ? $"/api/v2/courses?page={page + 1}&pageSize={pageSize}"
                    : (string?)null,
                prev = hasPrevious
                    ? $"/api/v2/courses?page={page - 1}&pageSize={pageSize}"
                    : (string?)null,
                enroll = "/api/v2/enrollments"
            }
        });
    }

    [HttpGet("search")]
    [EnableRateLimiting("search")]
    public async Task<IActionResult> SearchCourses(
        [FromQuery] string? term, CancellationToken ct)
    {
        var allCourses = await cachedCourseService.GetAllCoursesAsync(ct);
        if (!string.IsNullOrWhiteSpace(term))
        {
            allCourses = allCourses
                .Where(c => c.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                            c.Code.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        return Ok(allCourses);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCourse(
        int id, [FromBody] UpdateCourseRequest request, CancellationToken ct)
    {
        var updated = await courseService.UpdateTitleAsync(id, request.Title, ct);
        if (updated is null)
        {
            return NotFound();
        }

        await cachedCourseService.InvalidateCourseCacheAsync(ct);
        return Ok(updated);
    }
}
