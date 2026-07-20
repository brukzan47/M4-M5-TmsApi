using TmsApi.Entities;

namespace TmsApi.Services;

public interface IEnrollmentService
{
    Task<Enrollment> EnrollAsync(int studentId, int courseId, CancellationToken ct = default);
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
