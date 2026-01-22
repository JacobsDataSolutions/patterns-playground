// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using JDS.PollingDashboard1.Abstractions.Clock;
using JDS.PollingDashboard1.Abstractions.Jobs;
using JDS.PollingDashboard1.Services.Clock;
using JDS.PollingDashboard1.Services.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace JDS.PollingDashboard1.Services;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) =>
        services
        .AddSingleton<IDateTimeService, DateTimeService>()
        .AddSingleton<IJobsService, JobsService>();
}
