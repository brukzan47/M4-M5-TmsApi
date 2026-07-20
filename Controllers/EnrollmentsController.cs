using Microsoft.AspNetCore.Mvc;
using TmsApi.Entities;
using TmsApi.Models;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/enrollments")]
public sealed class EnrollmentsController(IEnrollmentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Enrollment>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Enrollment>> GetById(int id, CancellationToken ct)
    {
        var record = await service.GetByIdAsync(id, ct);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpPost]
    public async Task<ActionResult<Enrollment>> Create(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken ct)
    {
        var record = await service.EnrollAsync(request.StudentId, request.CourseId, ct);
        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
