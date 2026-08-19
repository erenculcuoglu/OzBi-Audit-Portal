using System.Threading.Tasks;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public interface ISlackNotificationService
    {
        Task SendLoginNotificationAsync(string fullName, string email, string role, string ipAddress, string? userAgent = null);
        Task SendTenantUserLoginNotificationAsync(string tenantName, string fullName, string email, int totalLoginCount);
        Task<bool> SendCustomerFeedbackNotificationAsync(CustomerFeedbackSlackPayload payload);
    }
}
