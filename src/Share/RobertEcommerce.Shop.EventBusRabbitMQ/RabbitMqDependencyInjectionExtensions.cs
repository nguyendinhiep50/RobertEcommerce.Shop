﻿using Microsoft.Extensions.DependencyInjection;
using RobertEcommerce.Shop.EventBusRabbitMQ;
using RobertEcommerce.Shop.EventBusRabbitMQ.EventBusRabbitMQ;

namespace Microsoft.Extensions.Hosting;

public static class RabbitMqDependencyInjectionExtensions
{
	private const string SectionName = "EventBus";

	public static IEventBusBuilder AddRabbitMqEventBus(this IHostApplicationBuilder builder, string connectionName)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.AddRabbitMQClient(connectionName, configureConnectionFactory: factory =>
		{
			((ConnectionFactory)factory).DispatchConsumersAsync = true;
		});

		// RabbitMQ.Client doesn't have built-in support for OpenTelemetry, so we need to add it ourselves
		builder.Services.AddOpenTelemetry()
		   .WithTracing(tracing =>
		   {
			   tracing.AddSource(RabbitMQTelemetry.ActivitySourceName);
		   });

		// Options support
		builder.Services.Configure<EventBusOptions>(builder.Configuration.GetSection(SectionName));

		// Abstractions on top of the core client API
		builder.Services.AddSingleton<RabbitMQTelemetry>();
		builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
		// Start consuming messages as soon as the application starts
		builder.Services.AddSingleton<IHostedService>(sp => (RabbitMQEventBus)sp.GetRequiredService<IEventBus>());

		return new EventBusBuilder(builder.Services);
	}

	private class EventBusBuilder(IServiceCollection services) : IEventBusBuilder
	{
		public IServiceCollection Services => services;
	}
}
