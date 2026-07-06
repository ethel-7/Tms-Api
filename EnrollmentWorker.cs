using TmsApi.Services;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        // Legacy code - kept for M4 compatibility
        // using var scope = _scopeFactory.CreateScope();
        // var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
    }
}
