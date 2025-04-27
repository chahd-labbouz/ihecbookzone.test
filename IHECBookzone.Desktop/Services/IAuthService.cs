using System;
using System.Threading.Tasks;
using IHECBookzone.Desktop.Models;

namespace IHECBookzone.Desktop.Services
{
    public interface IAuthService
    {
        event EventHandler<User> UserLoggedIn;
        event EventHandler UserLoggedOut;

        bool IsAuthenticated { get; }
        User CurrentUser { get; }
        string SessionToken { get; }

        Task<bool> SignInAsync(string email, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<bool> SignUpAsync(string email, string password, string fullName, string phoneNumber, 
            string levelOfStudy, string fieldOfStudy);
        void SignOut();
        Task<bool> ResetPasswordAsync(string email);
    }
} 