// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using Bogus;
using System.Net;
using JDS.PollingDashboard1.Abstractions.RunningJobs;

namespace JDS.PollingDashboard1.WebApi.RunningJobs;

public static class RunningJobsEndpoints
{
    private static readonly Random _random = new();
    private static readonly Faker _faker = new();

    public static IEndpointRouteBuilder MapRunningJobsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("running-jobs", GetRunningJobs).WithName(nameof(GetRunningJobs));
        builder.MapPost("running-jobs/run", RunJob).WithName(nameof(RunJob));
        return builder;
    }

    public static async Task<IResult> GetAllJobs()
    {
        int numJobs = _random.Next(25, 50);
        return Results.Ok(from _ in Enumerable.Range(0, numJobs) select new JobDto { Id = Guid.NewGuid(), Name = string.Join(" ", _faker.Lorem.Words(5)) });
    }

    public static async Task<IResult> GetRunningJobs(
        HttpContext httpContext,
        IRunningJobsService runningJobsService,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(runningJobsService, nameof(runningJobsService));
        (IReadOnlyList<RunningJob>? runningJobs, string? eTag, DateTime serverTimeUtc) = await runningJobsService.GetRunningJobsListAsync(cancellationToken);
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
        IRunningJobsService runningJobsService,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(runningJobsService, nameof(runningJobsService));
        (bool startedSuccessfully, DateTime? alreadyRunningSinceUtc) = await runningJobsService.AttemptJobRunAsync(jobId, cancellationToken);
        if (!startedSuccessfully)
        {
            return Results.Conflict(new AlreadyRunningDto { Message = $"Job {jobId} is already running.", AlreadyRunningSinceUtc = (DateTime)alreadyRunningSinceUtc! });
        }
        try
        {
            // Simulate long-running job.
            int runtime = _random.Next(10, 30) * 1000;
            await Task.Delay(runtime, cancellationToken);
            return Results.Ok();
        }
        finally
        {
            await runningJobsService.ClearJobRunningAsync(jobId, cancellationToken);
        }
    }
}
