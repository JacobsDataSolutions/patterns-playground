// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

namespace JDS.PollingDashboard1.Abstractions.RunningJobs;

public sealed record class AttemptRetryResult(bool StartedSuccessfully, DateTime? AlreadyRunningSinceUtc);
