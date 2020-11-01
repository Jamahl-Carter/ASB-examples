using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sync_adapter.producer.Services;

namespace sync_adapter.producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IRequestReplyService _requestReplyService;

        public TestController(IRequestReplyService requestReplyService) => _requestReplyService = requestReplyService;

        [HttpGet("{waitForReply}")]
        public async Task<IActionResult> GetAsync(bool waitForReply)
        {
            object dummyMessage = new { Content = Guid.NewGuid() };
            string result = await _requestReplyService.ExecuteRequestReplyOverSyncAsync(dummyMessage, waitForReply);

            return Ok(result);
        }

        [HttpGet("batch/{numToSend}")]
        public async Task<IActionResult> GetAsync(int numToSend)
        {
            var tasks = new List<Task>();
            string batchCorrelation = Guid.NewGuid().ToString();

            // Send messages in batches of 100
            for (int i = 0; i < numToSend; i++)
            {
                tasks.Add(_requestReplyService.ExecuteRequestReplyOverSyncAsync($"{batchCorrelation}: Message number: {i + 1}"));

                if (tasks.Count > 100) // 100 at a time
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }

            await Task.WhenAll(tasks);

            return Ok($"Batch of {numToSend} has completed request/reply cycles.");
        }
    }
}
