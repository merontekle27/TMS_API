using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Services;

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

    public Task<Course?> GetByCodeAsync(string code, CancellationToken ct) =>
        context.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Code == code, ct);

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct) =>
        await context.Courses
            .Include(c => c.Enrollments)
            .OrderBy(c => c.Title)
            .ToListAsync(ct);

    public async Task<Course?> UpdateTitleAsync(int id, string title, CancellationToken ct)
    {
        var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (course is null) return null;
        course.Title = title;
        await context.SaveChangesAsync(ct);
        return course;
    }

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
        IQueryable<Course> query = context.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.Title, searchPattern) ||
                EF.Functions.ILike(c.Code, searchPattern));
        }

        var totalCount = await query.CountAsync(ct);

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

        return new PagedResponse<CourseResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
