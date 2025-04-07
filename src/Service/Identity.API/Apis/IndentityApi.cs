using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.apis;
public static class IdentityApi
{
	public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder app)
	{
		var apiVersionSet = app.NewApiVersionSet()
			.HasApiVersion(1, 0)
			.ReportApiVersions()
			.Build();

		var api = app.MapGroup("api/identity")
			.WithApiVersionSet(apiVersionSet)
			.HasApiVersion(1, 0);

		#region Get
		api.MapGet("/admin", GetListUserByAdmin)
			.WithName("GetListUserByAdmin");

		api.MapGet("/admin/{userId}", GetUserByAdmin)
			.WithName("GetUserByAdmin");

		api.MapGet("/client/{userId}", GetUserByClient)
			.WithName("GetUserByClient");

		api.MapGet("/by-token/{token}", GetUserByToken)
			.WithName("GetUserByToken");

		api.MapGet("/by-email/{email}", GetUserByEmail)
			.WithName("GetUserByEmail");

		api.MapGet("/roles/{email}", GetRolesByEmail)
			.WithName("GetRolesByEmail");
		#endregion

		#region Create User
		api.MapPost("/create", CreateUser)
			.WithName("CreateUser");
		#endregion

		#region Update

		api.MapPut("/update", UpdateUser)
			.WithName("UpdateUser");

		api.MapPut("/update-password", UpdatePassword)
			.WithName("UpdatePassword");
		#endregion

		#region Delete

		api.MapDelete("/delete/{userId}", DeleteUser)
			.WithName("DeleteUser");
		#endregion

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

		#region Role Management
		api.MapPost("/add-roles", AddToRoles)
			.WithName("AddToRoles");

		api.MapPost("/remove-roles", RemoveFromRoles)
			.WithName("RemoveFromRoles");

		api.MapGet("/check-role", IsInRole)
			.WithName("IsInRole");
		#endregion

		#region OAuthService

		#endregion
		return app;
	}

	#region Get
	[ProducesResponseType<Ok<PaginatedItems<ApplicationUserDto>>>(StatusCodes.Status200OK, "application/json")]
	public static async Task<Ok<PaginatedItems<ApplicationUser>>> GetListUserByAdmin(
		[FromServices] IIdentityService services,
		[AsParameters] PaginationRequest paginationRequest)
	{
		var pageSize = paginationRequest.PageSize;
		var pageIndex = paginationRequest.PageIndex;

		var root = services.GetListUserByAdmin(
			filterExpression: null,
			sortExpressions: null,
			take: pageSize);

		var totalItems = await root
			.LongCountAsync();

		var itemsOnPage = await root
			.OrderBy(c => c.Name)
			.Skip(pageSize * pageIndex)
			.Take(pageSize)
			.ToListAsync();

		return TypedResults.Ok(new PaginatedItems<ApplicationUser>(pageIndex, pageSize, totalItems, itemsOnPage));
	}

	public static async Task<IResult> GetUserByAdmin(
		[FromServices] IIdentityService service,
		string userId)
	{
		var user = await service.GetUserByAdmin(userId).FirstOrDefaultAsync();
		return user is not null
			? Results.Ok(user)
			: Results.NotFound();
	}

	public static async Task<IResult> GetUserByClient(
		[FromServices] IIdentityService service,
		string userId)
	{
		var user = await service.GetUserByClient(userId).FirstOrDefaultAsync();
		return user is not null
			? Results.Ok(user)
			: Results.NotFound();
	}

	public static async Task<IResult> GetUserByToken(
		[FromServices] IIdentityService service,
		string token)
	{
		var user = await service.GetUserByToken(token).FirstOrDefaultAsync();
		return user is not null
			? Results.Ok(user)
			: Results.NotFound();
	}

	public static async Task<IResult> GetUserByEmail(
		[FromServices] IIdentityService service,
		string email)
	{
		var user = await service.GetByEmailAsync(email);
		return user is not null
			? Results.Ok(user)
			: Results.NotFound();
	}

	public static async Task<IResult> GetRolesByEmail(
		[FromServices] IIdentityService service, string email)
	{
		var user = await service.GetByEmailAsync(email);
		if (user is null) return Results.NotFound();
		var roles = await service.GetRolesAsync(user);
		return Results.Ok(roles);
	}
	#endregion

	#region Create User
	public static async Task<IResult> CreateUser(
		[FromServices] IIdentityService service,
		ApplicationUser user,
		string password)
	{
		var result = await service.CreateUserAsync(user, password);
		return result.Succeeded
			? Results.Ok()
			: Results.BadRequest(result.Errors);
	}
	#endregion

	#region Update
	public static async Task<IResult> UpdateUser(
		[FromServices] IIdentityService service,
		ApplicationUser user)
	{
		var result = await service.UpdateUserAsync(user);
		return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
	}

	public static async Task<IResult> UpdatePassword(
		[FromServices] IIdentityService service,
		string userId,
		string currentPassword,
		string newPassword)
	{
		var success = await service.UpdatePasswordAsync(userId, currentPassword, newPassword);
		return success
			? Results.Ok()
			: Results.BadRequest("Failed to update password");
	}
	#endregion

	#region Delete
	public static async Task<IResult> DeleteUser(
		[FromServices] IIdentityService service,
		string userId)
	{
		var result = await service.DeleteUserAsync(userId);
		return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
	}
	#endregion

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
