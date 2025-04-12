using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Apis;

public static class IndentiyForRoleApi
{
	public static IEndpointRouteBuilder MapIdentityForRoleApi(this IEndpointRouteBuilder app)
	{
		var apiVersionSet = app.NewApiVersionSet()
			.HasApiVersion(3, 0)
			.ReportApiVersions()
			.Build();

		var api = app.MapGroup("api/role-manager")
			.WithApiVersionSet(apiVersionSet)
			.HasApiVersion(1, 0);

		#region Role Management
		api.MapPost("/add-roles", AddToRoles)
			.WithName("AddToRoles");

		api.MapPost("/remove-roles", RemoveFromRoles)
			.WithName("RemoveFromRoles");

		api.MapGet("/check-role", IsInRole)
			.WithName("IsInRole");
		#endregion

		return app;
	}

	#region Role Management
	public static async Task<IResult> AddToRoles(
		[FromServices] IIdentityService service,
		ApplicationUser user,
		IEnumerable<string> roles)
	{
		var result = await service.AddToRolesAsync(user, roles);
		return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
	}

	public static async Task<IResult> RemoveFromRoles(
		[FromServices] IIdentityService service,
		ApplicationUser user,
		IEnumerable<string> roles)
	{
		var result = await service.RemoveFromRolesAsync(user, roles);
		return result.Succeeded
			? Results.Ok()
			: Results.BadRequest(result.Errors);
	}

	public static async Task<IResult> IsInRole(
		[FromServices] IIdentityService service,
		string email,
		string role)
	{
		var user = await service.GetByEmailAsync(email);
		if (user is null) return Results.NotFound();
		var isInRole = await service.IsInRoleAsync(user, role);
		return Results.Ok(isInRole);
	}

	#endregion
}
