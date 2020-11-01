using System.Threading.Tasks;

namespace sync_adapter.producer.Services
{
    public interface IRequestReplyService
    {
        Task<string> ExecuteRequestReplyOverSyncAsync(object content, bool waitForReply = true);
    }
}
