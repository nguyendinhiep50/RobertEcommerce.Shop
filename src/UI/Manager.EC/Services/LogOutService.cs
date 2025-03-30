﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Manager.EC.Services;
public class LogOutService
{
	public async Task LogOutAsync(HttpContext httpContext)
	{
		await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		await httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
	}
}
