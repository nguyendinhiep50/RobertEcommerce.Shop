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

		api.MapGet("/items/by", GetItemsByIds)
			.WithName("GetItemsByIds")
			.WithSummary("Batch get catalog items")
			.WithDescription("Get multiple items from the catalog")
			.WithTags("Items");

		api.MapGet("/GetListUserByAdmin", GetListUserByAdmin)
			.WithName("GetListUserByAdmin")
			.WithSummary("Get list user by admin")
			.WithDescription("Admin want get list us")
			.WithTags("Get");

		return app;
	}

	[ProducesResponseType<List<LoginRequest>>(StatusCodes.Status200OK, "application/json")]
	public static async Task<Results<Ok<List<LoginRequest>>, NotFound>> GetItemsByIds([FromQuery] int[] ids)
	{
		if (ids == null || ids.Length == 0)
		{
			return TypedResults.NotFound();
		}

		var items = ids.Select(id => new LoginRequest { UserName = $"User{id}", Password = "123" }).ToList();

		return TypedResults.Ok(items);
	}

	[ProducesResponseType<Ok<PaginatedItems<ApplicationUserDto>>>(StatusCodes.Status200OK, "application/json")]
	public static async Task<Ok<PaginatedItems<ApplicationUserDto>>> GetListUserByAdmin(
		[FromServices] IIdentityService services,
		[AsParameters] PaginationRequest paginationRequest)
	{
		var pageSize = paginationRequest.PageSize;
		var pageIndex = paginationRequest.PageIndex;

		var root = (IQueryable<ApplicationUserDto>)services.GetListUserByAdmin();

		var totalItems = await root
			.LongCountAsync();

		var itemsOnPage = await root
			.OrderBy(c => c.Name)
			.Skip(pageSize * pageIndex)
			.Take(pageSize)
			.ToListAsync();

		return TypedResults.Ok(new PaginatedItems<ApplicationUserDto>(pageIndex, pageSize, totalItems, itemsOnPage));
	}
}
