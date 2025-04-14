using System.Text.Json;

namespace Identity.API.Extensions;

public static class EnvironmentSettingsConfiguration
{
	public static IConfiguration MergeEnvironmentSettings(IConfiguration configuration)
	{
		var inMemorySettings = new Dictionary<string, string>();

		ConfigureJwtSettingsFromEnvironment(inMemorySettings);
		ConfigureAuthenticationSettingsFromEnvironment(inMemorySettings);

		if (inMemorySettings != null && inMemorySettings.Count > 0)
		{
			var memoryConfig = new ConfigurationBuilder()
				.AddInMemoryCollection(inMemorySettings)
				.Build();

			configuration = new ConfigurationBuilder()
				.AddConfiguration(memoryConfig)
				.AddConfiguration(configuration)
				.Build();
		}

		var jwtSection = configuration.GetSection(Constant.ENVIROMENT_APP_HOST_JWT);
		JwtSettings.Initialize(jwtSection);

		return configuration;
	}

	private static void ConfigureJwtSettingsFromEnvironment(Dictionary<string, string> settings)
	{
		var jwtJson = Environment.GetEnvironmentVariable(Constant.ENVIROMENT_APP_HOST_JWT);

		if (!string.IsNullOrEmpty(jwtJson))
		{
			using var doc = JsonDocument.Parse(jwtJson);
			FlattenJsonElement(doc.RootElement, Constant.ENVIROMENT_APP_HOST_JWT, settings);
		}
	}

	private static void ConfigureAuthenticationSettingsFromEnvironment(Dictionary<string, string> settings)
	{
		var authJson = Environment.GetEnvironmentVariable(Constant.ENVIROMENT_APP_HOST_AUTHENCATION);

		if (!string.IsNullOrEmpty(authJson))
		{
			using var doc = JsonDocument.Parse(authJson);
			FlattenJsonElement(doc.RootElement, Constant.ENVIROMENT_APP_HOST_AUTHENCATION, settings);
		}
	}

	private static void FlattenJsonElement(JsonElement element, string prefix, Dictionary<string, string> output)
	{
		switch (element.ValueKind)
		{
			case JsonValueKind.Object:
				foreach (var prop in element.EnumerateObject())
				{
					FlattenJsonElement(prop.Value, $"{prefix}:{prop.Name}", output);
				}
				break;

			case JsonValueKind.Array:
				int index = 0;
				foreach (var item in element.EnumerateArray())
				{
					FlattenJsonElement(item, $"{prefix}:{index}", output);
					index++;
				}
				break;

			default:
				output[prefix] = element.ToString() ?? string.Empty;
				break;
		}
	}
}
