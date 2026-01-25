// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Bogus;
using JDS.PollingDashboard1.Abstractions.Clock;
using JDS.PollingDashboard1.Abstractions.Jobs;
using Microsoft.Extensions.Caching.Distributed;

namespace JDS.PollingDashboard1.Services.Jobs;

internal sealed class JobsService : IJobsService, IDisposable
{
    private const string CacheKey = "running-jobs:v1";

    private static readonly Random _random = new();
    private static readonly Faker _faker = new();
    private static readonly ConcurrentDictionary<Guid, Job> _jobs;

    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly DistributedCacheEntryOptions _distributedCacheEntryOptions;
    private readonly IDateTimeService _dateTimeService;

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool isDisposed;

    static JobsService()
    {
        int numJobs = _random.Next(25, 50);
        _jobs = new ConcurrentDictionary<Guid, Job>((from n in Enumerable.Range(0, numJobs) select new Job(Guid.NewGuid(), string.Join(" ", $"{_faker.Lorem.Sentence(_random.Next(3, 5))}"), n, null, null)).ToDictionary(x => x.Id));
    }

    public JobsService(IDistributedCache cache, JsonSerializerOptions jsonSerializerOptions, IDateTimeService dateTimeService, TimeSpan? ttl = null)
    {
        ArgumentNullException.ThrowIfNull(cache, nameof(cache));
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions, nameof(jsonSerializerOptions));
        ArgumentNullException.ThrowIfNull(dateTimeService, nameof(dateTimeService));
        _cache = cache;
        _jsonSerializerOptions = jsonSerializerOptions;
        _dateTimeService = dateTimeService;
        _distributedCacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = ttl ?? TimeSpan.FromMinutes(30)
        };
    }

    public async Task<IReadOnlyList<Job>> GetJobs(CancellationToken cancellationToken = default) => [.. _jobs.Values.OrderBy(x => x.Number)];

    public async Task<Job> RunJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        Job updatedJob = _jobs[jobId] with { LastRunUtc = _dateTimeService.UtcNow };
        _jobs[jobId] = updatedJob;

        // Simulate long-running job.
        Task.Run(async () =>
        {
            int runtime = _random.Next(10, 30) * 1000;
            Thread.Sleep(runtime);
            updatedJob = _jobs[jobId] with { LastFinishedUtc = _dateTimeService.UtcNow };
            _jobs[jobId] = updatedJob;
            await ClearJobRunningAsync(jobId, cancellationToken);
        });

        return updatedJob;
    }

    public async Task<AttemptRetryResult> AttemptJobRunAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        DateTime kickedOffUtc = DateTime.UtcNow;
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            Dictionary<Guid, DateTime> runningJobMap = await GetRunningJobMapFromCacheAsync(cancellationToken);
            if (runningJobMap.TryGetValue(jobId, out DateTime runningSinceUtc))
            {
                return new(false, runningSinceUtc);
            }
            runningJobMap[jobId] = kickedOffUtc;
            await WriteRunningJobMapToCacheAsync(runningJobMap, cancellationToken);
            return new(true, null);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ClearJobRunningAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            Dictionary<Guid, DateTime> runningJobMap = await GetRunningJobMapFromCacheAsync(cancellationToken);
            if (!runningJobMap.Remove(jobId))
            {
                return;
            }
            if (runningJobMap.Count == 0)
            {
                await _cache.RemoveAsync(CacheKey, cancellationToken);
                return;
            }
            await WriteRunningJobMapToCacheAsync(runningJobMap, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<RunningJobsList> GetRunningJobsListAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            Dictionary<Guid, DateTime> runningJobMap = await GetRunningJobMapFromCacheAsync(cancellationToken);
            RunningJob[] jobs =
                [.. runningJobMap
                .OrderBy(job => job.Key)
                .Select(job => new RunningJob(job.Key, job.Value))];
            string eTagSource = string.Join("|", from j in jobs select $"{j.JobId}:{j.KickedOffUtc.Ticks}");
            string eTag = ComputeStrongETag(eTagSource);
            return new(jobs, eTag, _dateTimeService.UtcNow);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<Dictionary<Guid, DateTime>> GetRunningJobMapFromCacheAsync(CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await _cache.GetAsync(CacheKey, cancellationToken);
        if (bytes is null || bytes.Length == 0)
        {
            return new();
        }
        Dictionary<Guid, DateTime>? runningJobMap = JsonSerializer.Deserialize<Dictionary<Guid, DateTime>>(bytes, _jsonSerializerOptions);
        return runningJobMap ?? new();
    }

    private async Task WriteRunningJobMapToCacheAsync(Dictionary<Guid, DateTime> runningJobMap, CancellationToken cancellationToken = default)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(runningJobMap, _jsonSerializerOptions);
        await _cache.SetAsync(CacheKey, bytes, _distributedCacheEntryOptions, cancellationToken);
    }

    private static string ComputeStrongETag(string eTagSource)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(eTagSource));
        string hex = Convert.ToHexString(hash);
        return $"\"{hex}\"";
    }

    private void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                _semaphore.Dispose();
            }

            isDisposed = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
