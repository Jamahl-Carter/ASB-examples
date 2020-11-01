using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using sync_adapter.consumer.Configuration.Models;
using sync_adapter.consumer.Handler;
using System;

namespace sync_adapter.consumer.Configuration
{
    public static class Bindings
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddHostedService<RequestReplyHandler>(serviceProvider =>
            {
                // Use ASB configuration from env variable
                ServiceBusConnectionStringBuilder sendChannelBuilder = BuildConnectionString("RequestQueue", serviceProvider);
                ServiceBusConnectionStringBuilder receiveChannelBuilder = BuildConnectionString("ReplyQueue", serviceProvider);

                return new RequestReplyHandler(serviceProvider.GetRequiredService<ILogger<RequestReplyHandler>>(),
                    new QueueClient(sendChannelBuilder, ReceiveMode.PeekLock),
                    new QueueClient(receiveChannelBuilder, ReceiveMode.PeekLock));
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
