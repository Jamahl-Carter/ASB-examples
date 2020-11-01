using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using sync_adapter.producer.Configuration.Models;
using sync_adapter.producer.Handler;
using sync_adapter.producer.Services;
using sync_adapter.producer.Services.Implementation;
using System;

namespace sync_adapter.producer.Configuration
{
    public static class Bindings
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddHostedService<FallbackHandler>(serviceProvider =>
            {
                // Use ASB configuration from env variable
                ServiceBusConnectionStringBuilder receiveChannelBuilder = BuildConnectionString("ReplyQueue", serviceProvider);

                return new FallbackHandler(
                    serviceProvider.GetRequiredService<ILogger<FallbackHandler>>(), new QueueClient(receiveChannelBuilder, ReceiveMode.PeekLock));
            });
            services.AddTransient<IRequestReplyService, RequestReplyService>(serviceProvider =>
            {
                // Use ASB configuration from env variable
                ServiceBusConnectionStringBuilder sendChannelBuilder = BuildConnectionString("RequestQueue", serviceProvider);
                ServiceBusConnectionStringBuilder receiveChannelBuilder = BuildConnectionString("ReplyQueue", serviceProvider);

                return new RequestReplyService(
                    new QueueClient(sendChannelBuilder, ReceiveMode.PeekLock), 
                    new SessionClient(receiveChannelBuilder, ReceiveMode.PeekLock));
            });

            return services;
        }

        private static ServiceBusConnectionStringBuilder BuildConnectionString(string entityPath,
            IServiceProvider serviceProvider)
        {
            // Use ASB connection string from env variable
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var asbConfig = new AzureServiceBusConnection();
            configuration.Bind("AzureServiceBusConnection", asbConfig);

            return new ServiceBusConnectionStringBuilder(asbConfig.Endpoint, entityPath, asbConfig.SasKeyName,
                asbConfig.SasKey, asbConfig.TransportType);
        }
    }
}
