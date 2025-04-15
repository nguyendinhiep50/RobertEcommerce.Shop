using Microsoft.AspNetCore.Builder;
using RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiting(options =>
{
	options.Partitioner = context => context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
	options.DefaultLimiterFactory = key =>
		RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
		{
			PermitLimit = 10,
			Window = TimeSpan.FromSeconds(5),
			QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
			QueueLimit = 2
		});
});

var app = builder.Build();

app.UseRateLimiting();
