namespace Identity.API.Middlewares;

public static class MiddleWareExtensions
{
	public static IApplicationBuilder RegisterMiddlewares(this IApplicationBuilder builder)
	{
		builder.UseMiddleware<DeviceDetectionMiddleware>();
		return builder;
	}
}