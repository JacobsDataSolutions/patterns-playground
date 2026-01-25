// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using System.Net;
using JDS.PollingDashboard1.Abstractions.Jobs;

namespace JDS.PollingDashboard1.WebApi.Jobs;

public static class JobsEndpoints
{

    public static IEndpointRouteBuilder MapJobsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("jobs", GetAllJobs).WithName(nameof(GetAllJobs));
        builder.MapGet("jobs/running", GetRunningJobs).WithName(nameof(GetRunningJobs));
        builder.MapPost("jobs/run/{jobId}", RunJob).WithName(nameof(RunJob));
        return builder;
    }

    public static async Task<IResult> GetAllJobs(
        IJobsService runningJobsService,
        CancellationToken cancellationToken = default) => Results.Ok(from j in await runningJobsService.GetJobs(cancellationToken) select new JobDto { Id = j.Id, Name = j.Name, Number = j.Number, LastRunUtc = j.LastRunUtc, LastFinishedUtc = j.LastFinishedUtc });

    public static async Task<IResult> GetRunningJobs(
        HttpContext httpContext,
        IJobsService runningJobsService,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(runningJobsService, nameof(runningJobsService));
        (IReadOnlyList<RunningJob>? runningJobs, string? eTag, DateTime serverTimeUtc) = await runningJobsService.GetRunningJobsListAsync(cancellationToken);
        httpContext.Response.Headers.Location = "none";
        httpContext.Response.Headers.CacheControl = "no-store";
        httpContext.Response.Headers.ETag = eTag;
        string ifNoneMatch = httpContext.Request.Headers.IfNoneMatch.ToString();
        if (!string.IsNullOrWhiteSpace(ifNoneMatch) && string.Equals(ifNoneMatch, eTag, StringComparison.Ordinal))
        {
            return Results.StatusCode((int)HttpStatusCode.NotModified);
        }
        return Results.Ok(new RunningJobsDto { RunningJobs = runningJobs, ServerTimeUtc = serverTimeUtc });
    }

    public static async Task<IResult> RunJob(
        Guid jobId,
        IJobsService runningJobsService,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(runningJobsService, nameof(runningJobsService));
        (bool startedSuccessfully, DateTime? alreadyRunningSinceUtc) = await runningJobsService.AttemptJobRunAsync(jobId, cancellationToken);
        if (!startedSuccessfully)
        {
            return Results.Conflict(new AlreadyRunningDto { Message = $"Job {jobId} is already running.", AlreadyRunningSinceUtc = (DateTime)alreadyRunningSinceUtc! });
        }
        Job updatedJob = await runningJobsService.RunJobAsync(jobId, cancellationToken);
        return Results.Ok(new JobDto { Id = updatedJob.Id, Name = updatedJob.Name, Number = updatedJob.Number, LastRunUtc = updatedJob.LastRunUtc, LastFinishedUtc = updatedJob.LastFinishedUtc });
    }
}
