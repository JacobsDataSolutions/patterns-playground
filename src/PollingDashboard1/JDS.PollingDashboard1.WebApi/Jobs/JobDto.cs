// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

namespace JDS.PollingDashboard1.WebApi.Jobs;

public sealed class JobDto
{
    public int Number { get; init; }

    public Guid Id { get; init; }

    public required string Name { get; init; }

    public DateTime? LastRunUtc { get; init; }

    public DateTime? LastFinishedUtc { get; init; }
}
