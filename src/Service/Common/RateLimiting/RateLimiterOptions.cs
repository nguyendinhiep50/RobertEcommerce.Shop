using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace RateLimiting;

public class RateLimiterOptions
{
	public Func<HttpContext, string> Partitioner { get; set; } = context =>
		context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

	public Func<string, RateLimitPartition<string>> DefaultLimiterFactory { get; set; } = key =>
		RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
		{
			PermitLimit = 10,
			Window = TimeSpan.FromSeconds(10),
			QueueLimit = 2,
			QueueProcessingOrder = QueueProcessingOrder.OldestFirst
		});
}
