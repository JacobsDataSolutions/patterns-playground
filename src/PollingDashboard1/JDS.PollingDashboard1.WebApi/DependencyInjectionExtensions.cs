// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using System.Text.Json;
using System.Text.Json.Serialization;
using JDS.PollingDashboard1.Abstractions.Clock;
using JDS.PollingDashboard1.Abstractions.RunningJobs;

namespace JDS.PollingDashboard1.WebApi;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services) =>
        services
        .AddDistributedMemoryCache()
        .AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
}
