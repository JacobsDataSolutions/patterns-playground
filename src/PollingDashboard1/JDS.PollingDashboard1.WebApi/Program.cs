// Copyright (c) 2026 Jacobs Data Solutions, LLC
// Licensed under the MIT License. See LICENSE file in the project root.

using JDS.PollingDashboard1.Services;
using JDS.PollingDashboard1.WebApi;
using JDS.PollingDashboard1.WebApi.Jobs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddCaching();
builder.Services.AddApplicationServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithExposedHeaders("ETag")
);

app.UseHttpsRedirection();

RouteGroupBuilder routeGroupBuilder = app.MapGroup("api");

routeGroupBuilder.MapJobsEndpoints();

await app.RunAsync();
