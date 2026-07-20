using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;

namespace TmsApi.Services;

public sealed class EnrollmentService(
    TmsDbContext db,
    ILogger<EnrollmentService> logger)
    : IEnrollmentService
{
    public async Task<Enrollment> EnrollAsync(int studentId, int courseId, CancellationToken ct = default)
    {
        var existing = await db.Enrollments
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.CourseId == courseId, ct);

        if (existing is not null)
        {
            logger.LogWarning(
                "Duplicate enrollment attempt StudentId {StudentId} CourseId {CourseId} EnrollmentId {EnrollmentId}",
                studentId, courseId, existing.Id);
            return existing;
        }

        var record = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };

        db.Enrollments.Add(record);
        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "Enrolled StudentId {StudentId} in CourseId {CourseId} EnrollmentId {EnrollmentId}",
            studentId, courseId, record.Id);

        return record;
    }

    public Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Enrollments
            .AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken ct = default) =>
        await db.Enrollments
            .AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.Course)
            .OrderByDescending(x => x.EnrolledAt)
            .ToListAsync(ct);

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var record = await db.Enrollments.FindAsync([id], ct);
        if (record is null)
        {
            logger.LogWarning("Enrollment {EnrollmentId} not found", id);
            return false;
        }

        db.Enrollments.Remove(record);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Deleted enrollment {EnrollmentId}", id);
        return true;
    }
}
