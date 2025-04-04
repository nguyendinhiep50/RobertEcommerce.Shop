﻿using Aspire.Hosting.Lifecycle;

namespace RobertEcommerce.Shop.ManagerProject.AppHost;

internal static class Extensions
{
	/// <summary>
	/// Adds a hook to set the ASPNETCORE_FORWARDEDHEADERS_ENABLED environment variable to true for all projects in the application.
	/// </summary>
	public static IDistributedApplicationBuilder AddForwardedHeaders(this IDistributedApplicationBuilder builder)
	{
		builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();
		return builder;
	}

	private class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
	{
		public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
		{
			foreach (var p in appModel.GetProjectResources())
			{
				p.Annotations.Add(new EnvironmentCallbackAnnotation(context =>
				{
					context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true";
				}));
			}

			return Task.CompletedTask;
		}
	}

	/// <summary>
	/// Configures eShop projects to use Ollama for text embedding and chat.
	/// </summary>
	public static IDistributedApplicationBuilder AddOllama(this IDistributedApplicationBuilder builder,
		IResourceBuilder<ProjectResource> catalogApi,
		IResourceBuilder<ProjectResource> webApp)
	{
		var ollama = builder.AddOllama("ollama")
			.WithDataVolume()
			.WithGPUSupport()
			.WithOpenWebUI();
		var embeddings = ollama.AddModel("embedding", "all-minilm");
		var chat = ollama.AddModel("chat", "llama3.1");

		catalogApi.WithReference(embeddings)
			.WithEnvironment("OllamaEnabled", "true")
			.WaitFor(embeddings);
		webApp.WithReference(chat)
			.WithEnvironment("OllamaEnabled", "true")
			.WaitFor(chat);

		return builder;
	}
}
