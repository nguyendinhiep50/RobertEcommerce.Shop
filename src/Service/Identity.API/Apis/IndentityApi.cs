using Asp.Versioning.Conventions;
using Identity.API.Identities.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.apis;
public static class IdentityApi
{
	public static IEndpointRouteBuilder MapIdentityApi(this IEndpointRouteBuilder app)
	{
		// Tạo tập phiên bản API
		var apiVersionSet = app.NewApiVersionSet()
			.HasApiVersion(1, 0)
			.ReportApiVersions()
			.Build();

		// Nhóm API có versioning
		var api = app.MapGroup("api/identity")
			.WithApiVersionSet(apiVersionSet)
			.HasApiVersion(1, 0);

		api.MapGet("/items/by", GetItemsByIds)
			.WithName("GetItemsByIds")
			.WithSummary("Batch get catalog items")
			.WithDescription("Get multiple items from the catalog")
			.WithTags("Items");

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
}
