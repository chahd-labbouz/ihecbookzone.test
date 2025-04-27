using System.Collections.Generic;
using System.Threading.Tasks;
using IHECBookzone.Desktop.Models;

namespace IHECBookzone.Desktop.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetCurrentUserAsync();
        Task<bool> UpdateUserProfileAsync(User user);
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<bool> DeleteUserAsync(string userId);
    }
} 