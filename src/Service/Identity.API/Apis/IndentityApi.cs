using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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

		#region OAuthService
		api.MapGet("/google-login", GoogleLogin)
			.WithName("GoogleLogin");
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
		[FromBody] UserCreateDto user,
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

	#region OAuthService
	public static async Task<IResult> GoogleLogin(HttpContext context, IIdentityService service)
	{
		var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		if (!result.Succeeded || result.Principal == null)
		{
			var properties = new AuthenticationProperties
			{
				RedirectUri = "/api/auth/google"
			};

			await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
			return Results.Empty;
		}

		var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
		var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

		if (string.IsNullOrEmpty(email))
		{
			return Results.BadRequest(new { Message = "Không lấy được email từ Google." });
		}


		return Results.Ok(new
		{
			Message = "Đăng ký bằng Google thành công",
		});
	}
	#endregion
}
