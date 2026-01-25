// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

namespace JDS.PollingDashboard1.Abstractions.Jobs;

public interface IJobsService
{
    Task<AttemptRetryResult> AttemptJobRunAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task ClearJobRunningAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Job>> GetJobs(CancellationToken cancellationToken = default);
    Task<RunningJobsList> GetRunningJobsListAsync(CancellationToken cancellationToken = default);
    Task<Job> RunJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}
