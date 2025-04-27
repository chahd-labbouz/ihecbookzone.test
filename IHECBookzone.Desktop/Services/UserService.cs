using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IHECBookzone.Desktop.Models;

namespace IHECBookzone.Desktop.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl = "https://libbpevezajvedxiebvv.supabase.co";
        private readonly string _supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxpYmJwZXZlemFqdmVkeGllYnZ2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDU1ODQyODgsImV4cCI6MjA2MTE2MDI4OH0.SdRKdN_jt-8GPiGUMZug_AAFtFEiAIJh4vGegqqZ3is";
        private readonly AuthService _authService;
        private User _currentUser;

        public UserService(AuthService authService = null)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_supabaseUrl);
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            
            _authService = authService;
            
            if (_authService?.IsAuthenticated == true)
            {
                _currentUser = _authService.CurrentUser;
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_authService.SessionToken}");
            }
        }

        public User CurrentUser 
        { 
            get => _currentUser;
            set => _currentUser = value;
        }

        public void SetAuthToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
        }

        public async Task<bool> UpdateProfileAsync(User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Id))
                return false;

            try
            {
                var updateData = new
                {
                    full_name = user.FullName,
                    phone_number = user.PhoneNumber,
                    level_of_study = user.LevelOfStudy,
                    field_of_study = user.FieldOfStudy,
                    updated_at = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(updateData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PatchAsync($"/rest/v1/profiles?id=eq.{user.Id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    // Update current user instance
                    _currentUser = user;
                    _currentUser.UpdatedAt = DateTime.Now;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            try
            {
                var changePasswordRequest = new
                {
                    user_id = userId,
                    current_password = currentPassword,
                    new_password = newPassword
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(changePasswordRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/auth/v1/user/change-password", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return false;
            }
        }

        public async Task<string> UploadProfileImageAsync(string userId, byte[] imageData, string fileName)
        {
            if (string.IsNullOrEmpty(userId) || imageData == null || imageData.Length == 0)
                return null;

            try
            {
                // In a real implementation, this would use the Supabase Storage API
                // For this example, we'll simulate a successful upload and return a URL
                
                // Simulate API call delay
                await Task.Delay(1000);
                
                // Return a simulated avatar URL
                return $"https://example.com/avatars/{userId}/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading profile image: {ex.Message}");
                return null;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_supabaseUrl}/rest/v1/profiles?select=*");
                
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    return users?.Select(MapToUser).ToList() ?? new List<User>();
                }
                
                return new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var userDto = new
                {
                    full_name = user.FullName,
                    phone_number = user.PhoneNumber,
                    level_of_study = user.LevelOfStudy,
                    field_of_study = user.FieldOfStudy,
                    is_admin = user.IsAdmin
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(userDto),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PatchAsync($"{_supabaseUrl}/rest/v1/profiles?id=eq.{user.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/rest/v1/profiles?id=eq.{userId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/rest/v1/profiles?id=eq.{id}&select=*");
                
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    if (users != null && users.Count > 0)
                    {
                        return MapToUser(users[0]);
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user by ID: {ex.Message}");
                return null;
            }
        }
        
        public async Task<User> GetCurrentUserAsync()
        {
            if (_authService != null && _authService.IsAuthenticated)
            {
                return _authService.CurrentUser;
            }
            return null;
        }
        
        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            return await UpdateProfileAsync(user);
        }
        
        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (_authService != null && _authService.IsAuthenticated)
            {
                return await ChangePasswordAsync(_authService.CurrentUser.Id, currentPassword, newPassword);
            }
            return false;
        }

        private User MapToUser(UserDto dto)
        {
            return new User
            {
                Id = dto.Id,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                LevelOfStudy = dto.LevelOfStudy,
                FieldOfStudy = dto.FieldOfStudy,
                AvatarUrl = dto.AvatarUrl,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                IsAdmin = dto.IsAdmin
            };
        }

        private class UserDto
        {
            public string Id { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string LevelOfStudy { get; set; }
            public string FieldOfStudy { get; set; }
            public string AvatarUrl { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public bool IsAdmin { get; set; }
        }
    }
} 