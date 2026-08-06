using System.Threading.Tasks;

namespace OzBiPortalCRM.Services
{
    public interface ISlackNotificationService
    {
        Task SendLoginNotificationAsync(string fullName, string email, string role, string ipAddress, string? userAgent = null);
    }
}
