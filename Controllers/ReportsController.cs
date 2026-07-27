using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(TmsDbContext db) : ControllerBase
{
    [HttpGet("top-courses")]
    public async Task<IActionResult> GetTopCourses(CancellationToken ct)
    {
        var result = await db.Enrollments
            .AsNoTracking()
            .GroupBy(x => new { x.CourseId, x.Course.Title })
            .Select(g => new
            {
                g.Key.CourseId,
                g.Key.Title,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync(ct);

        return Ok(result);
    }
}
