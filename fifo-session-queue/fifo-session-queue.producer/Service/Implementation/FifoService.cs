using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace fifo_session_queue.producer.Service.Implementation
{
    internal class FifoService : IFifoService
    {
        private readonly ILogger<FifoService> _logger;
        private readonly IQueueClient _queueClient; // session enabled

        public FifoService(ILogger<FifoService> logger, IQueueClient queueClient)
        {
            _logger = logger;
            _queueClient = queueClient;
        }

        public async Task SendOrderedMessagesAsync(string[] messages, bool sameSession)
        {
            // Create unique session
            string sessionId = Guid.NewGuid().ToString();

            // Send messages in order
            foreach (string content in messages)
            {
                // Arrange message
                var message = new Message
                {
                    SessionId = sameSession ? sessionId : Guid.NewGuid().ToString(), // use same session if required
                    Body = Encoding.UTF8.GetBytes(content)
                };

                // Add message to queue
                await _queueClient.SendAsync(message);

                _logger.LogInformation($"Added msg to queue with content: {content}");
            }
        }
    }
}
