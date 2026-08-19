using System.Collections.Generic;
using System.Threading.Tasks;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public interface IOzBiFeedbackMonitorService
    {
        Task<int> CheckAndPushNewFeedbacksAsync(bool pushAllUnpushed = false, string? triggeredBy = null);
        Task<bool> PushFeedbackByIdAsync(string messageId, string? pushedBy = null);
        Task<HashSet<string>> GetPushedMessageIdsAsync();
        Task<List<FeedbackPushSnapshot>> GetPushedSnapshotsAsync(int limit = 100);
    }
}
