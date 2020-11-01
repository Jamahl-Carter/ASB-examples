using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace sync_adapter.producer.Services.Implementation
{
    internal class RequestReplyService : IRequestReplyService, IAsyncDisposable
    {
        private readonly IQueueClient _requestClient; // doesn't have to be session enabled
        private readonly ISessionClient _replySessionClient; // only support session enabled queue

        public RequestReplyService(IQueueClient requestClient, ISessionClient replySessionClient)
        {
            _requestClient = requestClient;
            _replySessionClient = replySessionClient;
        }

        public async Task<string> ExecuteRequestReplyOverSyncAsync(object content, bool waitForReply = true)
        {
            // Set/lock session
            string replySessionId = Guid.NewGuid().ToString();
            IMessageSession session = await _replySessionClient.AcceptMessageSessionAsync(replySessionId); // lock session

            try
            {
                // Arrange payload
                string raw = JsonConvert.SerializeObject(content);
                var message = new Message
                {
                    Body = Encoding.UTF8.GetBytes(raw),
                    ReplyToSessionId = replySessionId // tell recipient to reply using this session ID
                };

                // Send request
                using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
                    await _requestClient.SendAsync(message);

                // Exit early if you don't want to wait for reply
                if (waitForReply == false)
                    return $"Successfully sent request with sessionId: {replySessionId}";

                // Receive reply
                Message reply = await session.ReceiveAsync(TimeSpan.FromSeconds(10)); // 10s timeout

                if (reply == null)
                    return $"Failed to get reply within timeout for session {replySessionId}";

                string response = Encoding.UTF8.GetString(reply.Body);
                await session.CompleteAsync(reply.SystemProperties.LockToken);

                return response;

            }
            finally
            {
                await session.CloseAsync(); // release exlusive lock
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _requestClient.CloseAsync();
            await _replySessionClient.CloseAsync();
        }
    }
}
