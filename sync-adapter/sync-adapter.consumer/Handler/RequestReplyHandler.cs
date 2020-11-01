using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sync_adapter.consumer.Handler
{
    internal class RequestReplyHandler : BackgroundService, IAsyncDisposable
    {
        private readonly ILogger<RequestReplyHandler> _logger;
        private readonly IQueueClient _incomingQueueClient;
        private readonly IQueueClient _outgoingQueueClient;

        public RequestReplyHandler(ILogger<RequestReplyHandler> logger, IQueueClient incomingQueueClient, IQueueClient outgoingQueueClient)
        {
            _logger = logger;
            _incomingQueueClient = incomingQueueClient;
            _outgoingQueueClient = outgoingQueueClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _incomingQueueClient.RegisterMessageHandler(async (request, cancellationToken) =>
            {
                // Parse message
                string replySessionId = request.ReplyToSessionId;
                string rawMessage = Encoding.UTF8.GetString(request.Body);

                // Process message
                // consider DB query or another service message
                string reply = $"ReplySession: '{request.ReplyToSessionId}'\nRawRequest: '{rawMessage}'";
                _logger.LogInformation(reply);

                // Respond to sender
                var replyMessage = new Message
                {
                    Body = Encoding.UTF8.GetBytes(reply),
                    SessionId = replySessionId,
                    CorrelationId = request.MessageId
                };

                await _outgoingQueueClient.SendAsync(replyMessage);
                await _incomingQueueClient.CompleteAsync(request.SystemProperties.LockToken);
            }, new MessageHandlerOptions(async args => _logger.LogError(args.Exception.Message))
            {
                AutoComplete = false,
                MaxConcurrentCalls = 100 // up to 1000
            });

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _incomingQueueClient.CloseAsync();
            await _outgoingQueueClient.CloseAsync();
        }
    }
}
