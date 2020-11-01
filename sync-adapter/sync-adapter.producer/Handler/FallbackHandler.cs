using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sync_adapter.producer.Handler
{
    public class FallbackHandler : BackgroundService
    {
        private readonly ILogger<FallbackHandler> _logger;
        private readonly IQueueClient _responseQueueClient;

        public FallbackHandler(ILogger<FallbackHandler> logger, IQueueClient responseQueueClient)
        {
            _logger = logger;
            _responseQueueClient = responseQueueClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _responseQueueClient.RegisterSessionHandler(async (session, message, cancellationToken) =>
            {
                try
                {
                    // Parse message
                    string rawMessage = Encoding.UTF8.GetString(message.Body);

                    // Process message
                    // consider alerting/logging or triggering exentual rollback, depending on use case.

                    _logger.LogInformation($"Fallback queue received message with sessionID: {session.SessionId}");

                    // Complete message
                    await session.CompleteAsync(message.SystemProperties.LockToken);
                }
                finally
                {
                    await session.CloseAsync();
                }
            }, new SessionHandlerOptions(async args => _logger.LogError(args.Exception, args.Exception.Message))
            {
                AutoComplete = false,
                MaxConcurrentSessions = 100 // up to 1000
            });

            return Task.CompletedTask;
        }
    }
}
