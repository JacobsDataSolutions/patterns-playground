// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using JDS.PollingDashboard1.Abstractions.RunningJobs;

namespace JDS.PollingDashboard1.WebApi.RunningJobs;

public sealed class RunningJobsDto
{
    public required IReadOnlyList<RunningJob> RunningJobs { get; init; }

    public DateTime ServerTimeUtc { get; init; }
}
