using fifo_session_queue.producer.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace fifo_session_queue.producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IFifoService _fifoService;

        public TestController(IFifoService fifoService) => _fifoService = fifoService;

        [HttpPost("{sameSession}")]
        public Task PostAsync([FromBody] string[] messages, bool sameSession) => _fifoService.SendOrderedMessagesAsync(messages, sameSession);
    }
}
