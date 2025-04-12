using Asp.Versioning.Conventions;

namespace Identity.API.Apis;

public static class IndentiyForRoleApi
{
	public static IEndpointRouteBuilder MapIdentityForRoleApi(this IEndpointRouteBuilder app)
	{
		var apiVersionSet = app.NewApiVersionSet()
			.HasApiVersion(2, 0)
			.ReportApiVersions()
			.Build();

		return app;
	}
}
