// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using JDS.PollingDashboard1.Abstractions.Clock;

namespace JDS.PollingDashboard1.Services.Clock;

internal sealed class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
