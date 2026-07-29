using System.Collections.Generic;
using System.Threading.Tasks;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public interface IUserService
    {
        Task SeedDefaultUserAsync();
        Task<PortalUser?> AuthenticateAsync(string email, string password);
        Task<List<PortalUser>> GetAllUsersAsync();
        Task<PortalUser?> GetUserByIdAsync(int id);
        Task<(bool Success, string Message)> CreateUserAsync(PortalUser user, string plainPassword);
        Task<(bool Success, string Message)> UpdateUserAsync(PortalUser user, string? newPassword = null);
        Task<bool> DeleteUserAsync(int id);
    }
}
