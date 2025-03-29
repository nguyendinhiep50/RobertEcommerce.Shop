using RobertEcommerce.Shop.ManagerProject.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var redis = builder.AddRedis("redisEcommerce")
    .WithBindMount(@"D:\Docker_volumes\ecommerce_shop", "/var/lib/redis")
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitMq = builder.AddRabbitMQ("eventbusEcommerce")
    .WithBindMount(@"D:\Docker_volumes\ecommerce_shop", "/var/lib/rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("postgresEcommerce")
    .WithBindMount(@"D:\Docker_volumes\ecommerce_shop", "/var/lib/postgresql/data")
    .WithPgWeb(container => container
        .WithBindMount(@"D:\Docker_volumes\ecommerce_shop", "/var/lib/pgadmin")
        .WithLifetime(ContainerLifetime.Persistent))
    .WithImageTag("latest")
    .WithImage("ankane/pgvector")
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");
var identityDb = postgres.AddDatabase("identitydb");
var orderDb = postgres.AddDatabase("orderingdb");
var webhooksDb = postgres.AddDatabase("webhooksdb");

var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";

// Services
//var identityApi = builder.AddProject<Projects.Identity_API>("identity-api", launchProfileName)
//    .WithExternalHttpEndpoints()
//    .WithReference(identityDb);

//var identityEndpoint = identityApi.GetEndpoint(launchProfileName);

var basketApi = builder.AddProject<Projects.Manager_EC>("manager-ec")
    .WithReference(redis)
    .WithReference(rabbitMq).WaitFor(rabbitMq);
    //.WithEnvironment("Identity__Url", identityEndpoint);

builder.AddProject<Projects.Identity_API>("identity-api");
    //.WithEnvironment("Identity__Url", identityEndpoint);

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
