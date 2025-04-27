using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Utils;

namespace IHECBookzone.Desktop.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl = "https://libbpevezajvedxiebvv.supabase.co";
        private readonly string _supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxpYmJwZXZlemFqdmVkeGllYnZ2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDU1ODQyODgsImV4cCI6MjA2MTE2MDI4OH0.SdRKdN_jt-8GPiGUMZug_AAFtFEiAIJh4vGegqqZ3is";
        
        private User _currentUser;
        private string _sessionToken;

        public event EventHandler<User> UserLoggedIn;
        public event EventHandler UserLoggedOut;

        public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_sessionToken);
        public User CurrentUser => _currentUser;
        public string SessionToken => _sessionToken;

        public AuthService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.BaseAddress = new Uri(_supabaseUrl);
            
            // Increase timeout to handle potential network delays
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            // Ensure clean state on startup
            _currentUser = null;
            _sessionToken = null;
            
            // Initialize service availability check
            _ = CheckServiceAvailabilityAsync();
        }
        
        private async Task CheckServiceAvailabilityAsync()
        {
            // Check auth service availability on initialization
            await ServiceAvailabilityChecker.CheckAuthServiceAvailabilityAsync();
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckAuthServiceAvailabilityAsync())
                {
                    throw new Exception("Authentication service is not available.");
                }
                
                var authRequest = new
                {
                    email,
                    password
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(authRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/auth/v1/token?grant_type=password", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResult = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    _sessionToken = authResult.AccessToken;
                    
                    // Add authorization header for subsequent requests
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_sessionToken}");
                    
                    // Get user profile data
                    await GetUserProfileAsync(authResult.User.Id);
                    
                    UserLoggedIn?.Invoke(this, _currentUser);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                throw new Exception("Authentication service is not available.", ex);
            }
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckAuthServiceAvailabilityAsync())
                {
                    throw new Exception("Authentication service is not available.");
                }
                
                var signUpRequest = new
                {
                    email = user.Email,
                    password,
                    data = new
                    {
                        full_name = user.FullName,
                        phone_number = user.PhoneNumber,
                        level_of_study = user.LevelOfStudy,
                        field_of_study = user.FieldOfStudy
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(signUpRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/auth/v1/signup", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                throw new Exception("Authentication service is not available.", ex);
            }
        }

        public async Task<bool> SignUpAsync(string email, string password, string fullName, string phoneNumber, 
            string levelOfStudy, string fieldOfStudy)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckAuthServiceAvailabilityAsync())
                {
                    throw new Exception("Authentication service is not available.");
                }
                
                var signUpRequest = new
                {
                    email,
                    password,
                    data = new
                    {
                        full_name = fullName,
                        phone_number = phoneNumber,
                        level_of_study = levelOfStudy,
                        field_of_study = fieldOfStudy
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(signUpRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/auth/v1/signup", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign up error: {ex.Message}");
                throw new Exception("Authentication service is not available.", ex);
            }
        }

        public void SignOut()
        {
            _currentUser = null;
            _sessionToken = null;
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            UserLoggedOut?.Invoke(this, EventArgs.Empty);
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckAuthServiceAvailabilityAsync())
                {
                    throw new Exception("Authentication service is not available.");
                }
                
                var resetRequest = new
                {
                    email
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(resetRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/auth/v1/recover", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password reset error: {ex.Message}");
                throw new Exception("Authentication service is not available.", ex);
            }
        }

        private async Task GetUserProfileAsync(string userId)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckAuthServiceAvailabilityAsync())
                {
                    throw new Exception("Authentication service is not available.");
                }
                
                var response = await _httpClient.GetAsync($"/rest/v1/profiles?id=eq.{userId}&select=*");
                
                if (response.IsSuccessStatusCode)
                {
                    var profiles = await response.Content.ReadFromJsonAsync<ProfileResponse[]>();
                    if (profiles != null && profiles.Length > 0)
                    {
                        var profile = profiles[0];
                        _currentUser = new User
                        {
                            Id = userId,
                            Email = profile.Email,
                            FullName = profile.FullName,
                            PhoneNumber = profile.PhoneNumber,
                            LevelOfStudy = profile.LevelOfStudy,
                            FieldOfStudy = profile.FieldOfStudy,
                            AvatarUrl = profile.AvatarUrl,
                            CreatedAt = profile.CreatedAt,
                            UpdatedAt = profile.UpdatedAt,
                            IsAdmin = false // Set based on role check if needed
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user profile: {ex.Message}");
                throw new Exception("Authentication service is not available.", ex);
            }
        }

        private class AuthResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public string TokenType { get; set; }
            public int ExpiresIn { get; set; }
            public UserData User { get; set; }
        }

        private class UserData
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }

        private class ProfileResponse
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
        }
    }
} 