using Aspire.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RobertEcommerce.Shop.ManagerProject.AppHost;
using System.Text.Json;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var redis = builder.AddRedis("redisEcommerce")
	.WithBindMount(@"D:\Docker_volumes\ecommerce_shop\redis", "/var/lib/redis")
	.WithLifetime(ContainerLifetime.Persistent);

var rabbitMq = builder.AddRabbitMQ("eventbusEcommerce")
	.WithBindMount(@"D:\Docker_volumes\ecommerce_shop\rabbitmq", "/var/lib/rabbitmq")
	.WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("postgresEcommerce")
	.WithBindMount(@"D:\Docker_volumes\ecommerce_shop\postgres", "/var/lib/postgresql/data")
	.WithPgWeb(container => container
			 .WithBindMount(@"D:\Docker_volumes\ecommerce_shop\pgadmin", "/var/lib/pgadmin")
		.WithLifetime(ContainerLifetime.Persistent))
	.WithImageTag("0.16.2")
	.WithImage("ankane/pgvector")
	.WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");
var identityDb = postgres.AddDatabase("identitydb");
var orderDb = postgres.AddDatabase("orderingdb");
var webhooksDb = postgres.AddDatabase("webhooksdb");
var serviceCommonDb = postgres.AddDatabase("servicecommondb");

var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";

var jwtSettingsJson = builder.Configuration.ToJson("JwtSettings");
var authenticationSettingsJson = builder.Configuration.ToJson("Authentication");

var identityApi = builder.AddProject<Projects.Identity_API>("identity-api", launchProfileName)
	.WithExternalHttpEndpoints()
	.WithReference(identityDb)
	.WithEnvironment("JwtSettings", jwtSettingsJson)
	.WithEnvironment("Authentication", authenticationSettingsJson);

var basketApi = builder.AddProject<Projects.Manager_EC>("manager-ec")
	.WithReference(redis)
	.WithReference(rabbitMq).WaitFor(rabbitMq);

builder.Build().Run();

// For test use only.
// Looks for an environment variable that forces the use of HTTP for all the endpoints. We
// are doing this for ease of running the Playwright tests in CI.
static bool ShouldUseHttpForEndpoints()
{
	const string EnvVarName = "ESHOP_USE_HTTP_ENDPOINTS";
	var envValue = Environment.GetEnvironmentVariable(EnvVarName);

	// Attempt to parse the environment variable value; return true if it's exactly "1".
	return int.TryParse(envValue, out int result) && result == 1;
}
