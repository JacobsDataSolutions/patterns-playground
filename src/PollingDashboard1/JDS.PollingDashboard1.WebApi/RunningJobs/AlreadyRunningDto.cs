// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

namespace JDS.PollingDashboard1.WebApi.RunningJobs;

public sealed class AlreadyRunningDto
{
    public required string Message { get; init; }

    public DateTime AlreadyRunningSinceUtc { get; init; }
}
