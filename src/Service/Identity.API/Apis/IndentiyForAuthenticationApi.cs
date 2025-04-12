using Asp.Versioning.Conventions;

namespace Identity.API.Apis;

public static class IndentiyForAuthenticationApi
{
	public static IEndpointRouteBuilder MapIndentiyForAuthenticationApi(this IEndpointRouteBuilder app)
	{
		var apiVersionSet = app.NewApiVersionSet()
			.HasApiVersion(3, 0)
			.ReportApiVersions()
			.Build();

		return app;
	}
}
