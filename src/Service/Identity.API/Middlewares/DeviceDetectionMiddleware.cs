namespace Identity.API.Middlewares;

public class DeviceDetectionMiddleware
{
	private const string CONST_DEVICE_TYPE = "DeviceType";
	private readonly RequestDelegate _next;

	public DeviceDetectionMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var userAgent = context.Request.Headers["User-Agent"].ToString();

		if (!string.IsNullOrEmpty(userAgent))
		{
			if (IsSmartphone(userAgent))
			{
				context.Items[CONST_DEVICE_TYPE] = Constant.CONSTANT_KEY_DEVICE_LOGIN_MOBLIE;
			}
			else
			{
				context.Items[CONST_DEVICE_TYPE] = Constant.CONSTANT_KEY_DEVICE_LOGIN_PC;
			}
		}
		else
		{
			context.Items[CONST_DEVICE_TYPE] = Constant.CONSTANT_KEY_DEVICE_LOGIN_UNKNOW;
		}

		await _next(context);
	}

	private bool IsSmartphone(string userAgent)
	{
		return userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone");
	}
}
