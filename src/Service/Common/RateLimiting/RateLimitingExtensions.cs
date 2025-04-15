using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace RateLimiting;

public static class RateLimitingExtensions
{
	public static IServiceCollection AddRateLimiting(this IServiceCollection services, Action<RateLimiterOptions> configure)
	{
		services.AddSingleton<PartitionedRateLimiter<HttpContext>>(_ =>
		{
			var options = new RateLimiterOptions();
			configure(options);

			return PartitionedRateLimiter.Create<HttpContext, string>(
				context =>
				{
					var key = options.Partitioner(context);
					return options.DefaultLimiterFactory(key);
				}
			);
		});

		return services;
	}

	public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
	{
		return app.UseMiddleware<RateLimiterMiddleware>();
	}
}
