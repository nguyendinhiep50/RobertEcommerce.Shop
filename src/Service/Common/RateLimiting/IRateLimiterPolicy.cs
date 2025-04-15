using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace RateLimiting;

public interface IRateLimiterPolicy
{
	string GetPartitionKey(HttpContext context);

	RateLimitPartition<string> GetPartition(string key);
}
