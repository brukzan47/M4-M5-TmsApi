using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/m5")]
public sealed class M5OperationsController(
    TmsDbContext db,
    ILogger<M5OperationsController> logger)
    : ControllerBase
{
    // Exercise 7A: intentionally inefficient N+1 implementation.
    [HttpGet("n-plus-one")]
    public async Task<IActionResult> DemonstrateNPlusOne(
        CancellationToken cancellationToken)
    {
        var students = await db.Students
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .ToListAsync(cancellationToken);

        var report = new List<object>();

        foreach (var student in students)
        {
            // One additional SQL query for every student.
            int enrollmentCount = await db.Enrollments
                .AsNoTracking()
                .CountAsync(
                    enrollment => enrollment.StudentId == student.Id,
                    cancellationToken
                );

            report.Add(new
            {
                student.Id,
                student.Name,
                EnrollmentCount = enrollmentCount
            });
        }

        return Ok(report);
    }

    // Exercise 7B: fixed implementation; translated into one SQL query.
    [HttpGet("n-plus-one-fixed")]
    public async Task<IActionResult> GetEnrollmentReport(
        CancellationToken cancellationToken)
    {
        var report = await db.Students
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .Select(student => new
            {
                student.Id,
                student.Name,

                EnrollmentCount = student.Enrollments.Count
            })
            .ToListAsync(cancellationToken);

        return Ok(report);
    }

    // Exercise 9: one set-based SQL UPDATE.
    [HttpPost("archive-enrollments")]
    public async Task<IActionResult> ArchiveOldEnrollments(
        DateTime? cutoff,
        CancellationToken cancellationToken)
    {
        DateTime effectiveCutoff = cutoff
            ?? DateTime.UtcNow.AddYears(-1);

        int updatedRows = await db.Enrollments
            .Where(enrollment =>
                !enrollment.IsArchived &&
                enrollment.EnrolledAt < effectiveCutoff
            )
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    enrollment => enrollment.IsArchived,
                    true
                ),
                cancellationToken
            );

        logger.LogInformation(
            "Archived {EnrollmentCount} enrollments before {Cutoff}",
            updatedRows,
            effectiveCutoff
        );

        return Ok(new
        {
            cutoff = effectiveCutoff,
            archivedEnrollments = updatedRows
        });
    }

    [HttpGet("archived-enrollments")]
    public async Task<IActionResult> GetArchivedEnrollments(
        CancellationToken cancellationToken)
    {
        var enrollments = await db.Enrollments
            .AsNoTracking()
            .Where(enrollment => enrollment.IsArchived)
            .OrderByDescending(enrollment => enrollment.EnrolledAt)
            .Select(enrollment => new
            {
                enrollment.Id,
                enrollment.StudentId,
                enrollment.CourseId,
                enrollment.EnrolledAt,
                enrollment.IsArchived
            })
            .ToListAsync(cancellationToken);

        return Ok(enrollments);
    }
}