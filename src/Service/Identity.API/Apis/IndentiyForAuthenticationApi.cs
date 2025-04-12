using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Apis;

public static class IndentiyForAuthenticationApi
{
	public static IEndpointRouteBuilder MapIndentiyForAuthenticationApi(this IEndpointRouteBuilder app)
	{
		var apiVersionSet = app.NewApiVersionSet()
			.HasApiVersion(2, 0)
			.ReportApiVersions()
			.Build();

		var api = app.MapGroup("api/authencation")
			.WithApiVersionSet(apiVersionSet)
			.HasApiVersion(1, 0);

		#region Authentication

		api.MapPost("/authenticate", Authenticate)
			.WithName("Authenticate");

		api.MapPost("/validate-token", ValidateToken)
			.WithName("ValidateToken");
		#endregion

		#region Token
		api.MapPost("/generate-email-confirm", GenerateEmailConfirmToken)
			.WithName("GenerateEmailConfirmToken");

		api.MapPost("/confirm-email", ConfirmEmail)
			.WithName("ConfirmEmail");

		api.MapPost("/generate-password-reset", GeneratePasswordResetToken)
			.WithName("GeneratePasswordResetToken");

		api.MapPost("/reset-password", ResetPassword)
			.WithName("ResetPassword");
		#endregion

		return app;
	}

	#region Authentication
	public static async Task<IResult> Authenticate(
		[FromServices] IIdentityService service,
		string email,
		string password)
	{
		var token = await service.AuthenticateAsync(email, password);
		return token is not null
			? Results.Ok(token)
			: Results.Unauthorized();
	}
	#endregion

	#region Token
	public static async Task<IResult> ValidateToken(
		[FromServices] IIdentityService service,
		string token)
	{
		var valid = await service.ValidateTokenAsync(token);
		return Results.Ok(valid);
	}

	public static async Task<IResult> GenerateEmailConfirmToken(
		[FromServices] IIdentityService service,
		ApplicationUser user)
	{
		var token = await service.GenerateEmailConfirmationTokenAsync(user);
		return Results.Ok(token);
	}

	public static async Task<IResult> ConfirmEmail(
		[FromServices] IIdentityService service, string userId, string token)
	{
		var result = await service.ConfirmEmailAsync(userId, token);
		return result.Succeeded
			? Results.Ok()
			: Results.BadRequest(result.Errors);
	}

	public static async Task<IResult> GeneratePasswordResetToken(
		[FromServices] IIdentityService service,
		ApplicationUser user)
	{
		var token = await service.GeneratePasswordResetTokenAsync(user);
		return Results.Ok(token);
	}

	public static async Task<IResult> ResetPassword(
		[FromServices] IIdentityService service,
		string email,
		string token,
		string newPassword)
	{
		var result = await service.ResetPasswordAsync(email, token, newPassword);
		return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
	}
	#endregion

}
