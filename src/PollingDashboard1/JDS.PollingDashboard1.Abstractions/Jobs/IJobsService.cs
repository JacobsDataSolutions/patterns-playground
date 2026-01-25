
namespace JDS.PollingDashboard1.Abstractions.Jobs;

public interface IJobsService
{
    Task<AttemptRetryResult> AttemptJobRunAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task ClearJobRunningAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Job>> GetJobs(CancellationToken cancellationToken = default);
    Task<RunningJobsList> GetRunningJobsListAsync(CancellationToken cancellationToken = default);
    Task<Job> RunJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}
