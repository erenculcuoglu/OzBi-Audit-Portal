using System.Threading.Tasks;

namespace OzBiPortalCRM.Services
{
    public interface IOzBiLoginMonitorService
    {
        Task CheckForNewLoginsAsync();
    }
}
