using fifo_session_queue.consumer.Configuration.Models;
using fifo_session_queue.consumer.Handler;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace fifo_session_queue.consumer.Configuration
{
    public static class Bindings
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddHostedService<FifoQueueHandler>(serviceProvider =>
            {
                // Use ASB configuration from env variable
                ServiceBusConnectionStringBuilder fifoChannelBuilder = BuildConnectionString("FifoQueue", serviceProvider);

                return new FifoQueueHandler(
                    serviceProvider.GetRequiredService<ILogger<FifoQueueHandler>>(), 
                    new QueueClient(fifoChannelBuilder, ReceiveMode.PeekLock));
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
