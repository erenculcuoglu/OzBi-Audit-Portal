using System.Collections.Generic;
using System.Threading.Tasks;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public interface IOzBiSqlErrorMonitorService
    {
        Task<int> CheckAndPushNewSqlErrorsAsync(bool pushAllUnpushed = false, string? triggeredBy = null);
        Task<bool> PushSqlErrorByIdAsync(string messageId, string? pushedBy = null);
        Task<HashSet<string>> GetPushedSqlErrorMessageIdsAsync();
        Task<List<SqlErrorPushSnapshot>> GetPushedSnapshotsAsync(int limit = 100);
    }
}
