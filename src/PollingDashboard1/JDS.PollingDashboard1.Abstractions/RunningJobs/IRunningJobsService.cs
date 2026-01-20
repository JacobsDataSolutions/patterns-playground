namespace JDS.PollingDashboard1.Abstractions.RunningJobs;

public interface IRunningJobsService
{
    Task<AttemptRetryResult> AttemptJobRunAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task ClearJobRunningAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<RunningJobsList> GetRunningJobsListAsync(CancellationToken cancellationToken = default);
}
