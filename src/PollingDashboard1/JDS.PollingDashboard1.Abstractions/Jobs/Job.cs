// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

namespace JDS.PollingDashboard1.Abstractions.Jobs;

public sealed record class Job(Guid Id, string Name, int Number, DateTime? LastRunUtc, DateTime? LastFinishedUtc);
