using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using Tms.Api.Dtos;
using TmsApi.Services;
using System.Security.Cryptography;

public  class CourseService(TmsDbContext context, ILogger<CourseService> logger
) : ICourseService
{
    public Task<CourseResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        return context.Courses
            .AsNoTracking()
            .Where(course => course.Id == id)
            .Select(course => new CourseResponseDto(
                course.Id,
                course.Code,
                course.Title,
                course.MaxCapacity,
                course.Enrollments.Count
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

   public Task<bool> CodeExistsAsync(string code, CancellationToken ct) =>
context.Courses.AsNoTracking().AnyAsync(c => c.Code == code, ct);

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
}}