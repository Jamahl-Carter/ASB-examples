using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fifo_session_queue.consumer.Handler
{
    public class FifoQueueHandler : BackgroundService
    {
        private readonly ILogger<FifoQueueHandler> _logger;
        private readonly IQueueClient _queueClient;

        public FifoQueueHandler(ILogger<FifoQueueHandler> logger, IQueueClient queueClient)
        {
            _logger = logger;
            _queueClient = queueClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueClient.RegisterSessionHandler(async (session, message, cancellationToken) =>
            {
                // Parse message
                string rawMessage = Encoding.UTF8.GetString(message.Body);

                // Handle message
                // ..

                // Complete message
                await session.CompleteAsync(message.SystemProperties.LockToken);

                _logger.LogInformation($"Processed message with content: {rawMessage}");
            },
            new SessionHandlerOptions(async args => _logger.LogError(args.Exception.Message))
            {
                AutoComplete = false,
                MaxConcurrentSessions = 100 // up to 1000
            });

            return Task.CompletedTask;
        }
    }
}
