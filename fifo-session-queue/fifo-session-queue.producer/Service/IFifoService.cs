using System.Threading.Tasks;

namespace fifo_session_queue.producer.Service
{
    public interface IFifoService
    {
        Task SendOrderedMessagesAsync(string[] messages, bool sameSession);
    }
}
