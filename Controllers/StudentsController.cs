using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using TmsApi.Models;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/students")]
public sealed class StudentsController(
    TmsDbContext db,
    ILogger<StudentsController> logger)
    : ControllerBase
{
    // Normal query: IsDeleted students are hidden automatically.
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var students = await db.Students
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(student => new
            {
                student.Id,
                student.RegistrationNumber,
                student.Name,
                student.GPA,
                student.IsActive,
                student.Version,

                LastUpdated = EF.Property<DateTime>(
                    student,
                    "LastUpdated"
                )
            })
            .ToListAsync(cancellationToken);

        return Ok(students);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var student = await db.Students
            .AsNoTracking()
            .Where(student => student.Id == id)
            .Select(student => new
            {
                student.Id,
                student.RegistrationNumber,
                student.Name,
                student.GPA,
                student.IsActive,
                student.Version,

                LastUpdated = EF.Property<DateTime>(
                    student,
                    "LastUpdated"
                )
            })
            .FirstOrDefaultAsync(cancellationToken);

        return student is null
            ? NotFound()
            : Ok(student);
    }

    // Admin view: includes soft-deleted students.
    [HttpGet("admin/all")]
    public async Task<IActionResult> GetAllForAdmin(
        CancellationToken cancellationToken)
    {
        var students = await db.Students
            .IgnoreQueryFilters()
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .Select(student => new
            {
                student.Id,
                student.RegistrationNumber,
                student.Name,
                student.GPA,
                student.IsDeleted,
                student.Version
            })
            .ToListAsync(cancellationToken);

        return Ok(students);
    }

    // Optimistic-concurrency update.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        Student? student = await db.Students
            .FirstOrDefaultAsync(
                student => student.Id == id,
                cancellationToken
            );

        if (student is null)
        {
            return NotFound();
        }

        // Set the original concurrency version supplied by the client.
        db.Entry(student)
            .Property(item => item.Version)
            .OriginalValue = request.Version;

        student.Name = request.Name;
        student.GPA = request.GPA;

        try
        {
            await db.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                message = "Student updated successfully.",
                student.Id,
                student.Name,
                student.GPA,
                student.Version
            });
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogWarning(
                exception,
                "Concurrency conflict while updating student {StudentId}",
                id
            );

            return Conflict(new
            {
                title = "Concurrency conflict",
                status = StatusCodes.Status409Conflict,
                detail =
                    "This student was changed by another user. Reload the record and try again."
            });
        }
    }

    // Soft delete; no physical DELETE is sent.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDelete(
        int id,
        CancellationToken cancellationToken)
    {
        Student? student = await db.Students
            .FirstOrDefaultAsync(
                student => student.Id == id,
                cancellationToken
            );

        if (student is null)
        {
            return NotFound();
        }

        student.IsDeleted = true;

        await db.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // Admin restore.
    [HttpPost("{id:int}/restore")]
    public async Task<IActionResult> Restore(
        int id,
        CancellationToken cancellationToken)
    {
        Student? student = await db.Students
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                student => student.Id == id,
                cancellationToken
            );

        if (student is null)
        {
            return NotFound();
        }

        student.IsDeleted = false;

        await db.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            message = "Student restored.",
            student.Id
        });
    }
}