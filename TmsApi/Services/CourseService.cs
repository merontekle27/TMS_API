using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Dtos;
using TmsApi.Models;

namespace TmsApi.Services;

public class CourseService(TmsDbContext context, ILogger<CourseService> logger) : ICourseService
{
    public Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct) =>
        context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count))
            .FirstOrDefaultAsync(ct);

    public async Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct)
    {
        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("Created course {CourseId} ({Code})", course.Id, course.Code);

        return (await GetByIdAsync(course.Id, ct))!;
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct) =>
        context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.Code == code, ct);

    public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct)
    {
        // TODO 1: Start with a no-tracking IQueryable<Course>
        IQueryable<Course> query = context.Courses.AsNoTracking();

        // TODO 2: Case-insensitive search on Title or Code using ILike
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search}%";
            query = query.Where(c => EF.Functions.ILike(c.Title, searchPattern) || EF.Functions.ILike(c.Code, searchPattern));
        }

        // TODO 3: Count BEFORE paging
        var totalCount = await query.CountAsync(ct);

        // TODO 4: Apply OrderBy (whitelist: Title, Code, MaxCapacity)
        var orderBy = request.OrderBy?.Trim();
        query = (orderBy, request.Descending) switch
        {
            ("Code", false) => query.OrderBy(c => c.Code),
            ("Code", true) => query.OrderByDescending(c => c.Code),
            ("MaxCapacity", false) => query.OrderBy(c => c.MaxCapacity),
            ("MaxCapacity", true) => query.OrderByDescending(c => c.MaxCapacity),
            ("Title", true) => query.OrderByDescending(c => c.Title),
            _ => request.Descending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title)
        };

        // TODO 5: Skip, Take, Select projection, Materialise
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count))
            .ToListAsync(ct);

        // TODO 6: Return PagedResponse
        return new PagedResponse<CourseResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
