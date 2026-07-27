namespace TmsApi.Services;

public sealed class EnrollmentWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<EnrollmentWorker> logger)
{
    public async Task ProcessBatchAsync(CancellationToken ct = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
        var records = await service.GetAllAsync(ct);
        logger.LogInformation("Enrollment worker processed {Count} records", records.Count);
    }
}
