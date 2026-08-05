using Microsoft.EntityFrameworkCore;
using Tms.Api.Dtos;
using TmsApi.Data;
using TmsApi.Entities;

namespace TmsApi.Services;

public class CourseService(TmsDbContext context) : ICourseService
{
    public async Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto(c.Id, c.Code, c.Title, c.MaxCapacity, c.Enrollments.Count))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct)
    {
        return await context.Courses.AsNoTracking().AnyAsync(c => c.Code == code, ct);
    }

    public async Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct)
    {
        if (await CodeExistsAsync(request.Code, ct))
        {
            throw new InvalidOperationException($"A course with code '{request.Code}' already exists.");
        }

        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);

        return (await GetByIdAsync(course.Id, ct))!;
    }

    public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(PagedRequest request, CancellationToken ct)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = request.PageSize;

        IQueryable<Course> query = context.Courses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(c => EF.Functions.ILike(c.Title, $"%{search}%") || EF.Functions.ILike(c.Code, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(ct);

        IQueryable<Course> orderedQuery = request.OrderBy switch
        {
            "Code" => request.Descending ? query.OrderByDescending(c => c.Code) : query.OrderBy(c => c.Code),
            "MaxCapacity" => request.Descending ? query.OrderByDescending(c => c.MaxCapacity) : query.OrderBy(c => c.MaxCapacity),
            _ => request.Descending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title)
        };

        var items = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseResponseDto(c.Id, c.Code, c.Title, c.MaxCapacity, c.Enrollments.Count))
            .ToListAsync(ct);

        return new PagedResponse<CourseResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}