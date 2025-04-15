using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace RateLimiting;

public class RateLimiterMiddleware
{
	private readonly RequestDelegate _next;
	private readonly PartitionedRateLimiter<HttpContext> _rateLimiter;

	public RateLimiterMiddleware(RequestDelegate next, PartitionedRateLimiter<HttpContext> rateLimiter)
	{
		_next = next;
		_rateLimiter = rateLimiter;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		using var lease = await _rateLimiter.AcquireAsync(context, 1);

		if (!lease.IsAcquired)
		{
			context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
			await context.Response.WriteAsync("Rate limit exceeded.");
			return;
		}

		await _next(context);
	}
}
