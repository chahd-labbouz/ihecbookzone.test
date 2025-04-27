using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Diagnostics;

namespace IHECBookzone.Desktop.Utils
{
    public static class ServiceAvailabilityChecker
    {
        private static bool _authServiceAvailable = false;
        private static bool _bookServiceAvailable = false;
        private static bool _borrowingServiceAvailable = false;
        private static bool _userServiceAvailable = false;
        private static DateTime _lastAuthCheck = DateTime.MinValue;
        private static DateTime _lastBookCheck = DateTime.MinValue;
        private static DateTime _lastBorrowingCheck = DateTime.MinValue;
        private static DateTime _lastUserCheck = DateTime.MinValue;
        private static readonly TimeSpan _cacheTime = TimeSpan.FromSeconds(30); // Reduced cache time for more responsive feedback
        private static readonly string _supabaseUrl = "https://libbpevezajvedxiebvv.supabase.co";
        private static readonly string _supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxpYmJwZXZlemFqdmVkeGllYnZ2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDU1ODQyODgsImV4cCI6MjA2MTE2MDI4OH0.SdRKdN_jt-8GPiGUMZug_AAFtFEiAIJh4vGegqqZ3is";

        public static bool IsAuthServiceAvailable => _authServiceAvailable;
        public static bool IsBookServiceAvailable => _bookServiceAvailable;
        public static bool IsBorrowingServiceAvailable => _borrowingServiceAvailable;
        public static bool IsUserServiceAvailable => _userServiceAvailable;

        public static async Task<bool> CheckAllServicesAsync()
        {
            var authTask = CheckAuthServiceAvailabilityAsync();
            var bookTask = CheckBookServiceAvailabilityAsync();
            var borrowingTask = CheckBorrowingServiceAvailabilityAsync();
            var userTask = CheckUserServiceAvailabilityAsync();

            await Task.WhenAll(authTask, bookTask, borrowingTask, userTask);

            return _authServiceAvailable && _bookServiceAvailable && 
                   _borrowingServiceAvailable && _userServiceAvailable;
        }

        public static async Task<bool> CheckAuthServiceAvailabilityAsync()
        {
            // Check if we have a recent result cached
            if ((DateTime.Now - _lastAuthCheck) < _cacheTime)
            {
                return _authServiceAvailable;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.DefaultRequestHeaders.Add("apikey", _supabaseKey);

                    // Try with a more reliable endpoint
                    var response = await client.GetAsync($"{_supabaseUrl}/auth/v1/");
                    
                    bool success = response.IsSuccessStatusCode;
                    
                    Logger.Log(
                        success ? LogLevel.Info : LogLevel.Warning, 
                        $"Auth service check - Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                    
                    _authServiceAvailable = success;
                    _lastAuthCheck = DateTime.Now;
                    
                    return _authServiceAvailable;
                }
            }
            catch (Exception ex)
            {
                _authServiceAvailable = false;
                _lastAuthCheck = DateTime.Now;
                Logger.Log(LogLevel.Error, $"Auth service availability check failed: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> CheckBookServiceAvailabilityAsync()
        {
            // Check if we have a recent result cached
            if ((DateTime.Now - _lastBookCheck) < _cacheTime)
            {
                return _bookServiceAvailable;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.DefaultRequestHeaders.Add("apikey", _supabaseKey);
                    
                    var response = await client.GetAsync($"{_supabaseUrl}/rest/v1/books?limit=1");
                    
                    bool success = response.IsSuccessStatusCode;
                    
                    Logger.Log(
                        success ? LogLevel.Info : LogLevel.Warning, 
                        $"Book service check - Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                    
                    _bookServiceAvailable = success;
                    _lastBookCheck = DateTime.Now;
                    
                    return _bookServiceAvailable;
                }
            }
            catch (Exception ex)
            {
                _bookServiceAvailable = false;
                _lastBookCheck = DateTime.Now;
                Logger.Log(LogLevel.Error, $"Book service availability check failed: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> CheckBorrowingServiceAvailabilityAsync()
        {
            // Check if we have a recent result cached
            if ((DateTime.Now - _lastBorrowingCheck) < _cacheTime)
            {
                return _borrowingServiceAvailable;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.DefaultRequestHeaders.Add("apikey", _supabaseKey);
                    
                    var response = await client.GetAsync($"{_supabaseUrl}/rest/v1/borrowings?limit=1");
                    
                    bool success = response.IsSuccessStatusCode;
                    
                    Logger.Log(
                        success ? LogLevel.Info : LogLevel.Warning, 
                        $"Borrowing service check - Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                    
                    _borrowingServiceAvailable = success;
                    _lastBorrowingCheck = DateTime.Now;
                    
                    return _borrowingServiceAvailable;
                }
            }
            catch (Exception ex)
            {
                _borrowingServiceAvailable = false;
                _lastBorrowingCheck = DateTime.Now;
                Logger.Log(LogLevel.Error, $"Borrowing service availability check failed: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> CheckUserServiceAvailabilityAsync()
        {
            // Check if we have a recent result cached
            if ((DateTime.Now - _lastUserCheck) < _cacheTime)
            {
                return _userServiceAvailable;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.DefaultRequestHeaders.Add("apikey", _supabaseKey);
                    
                    var response = await client.GetAsync($"{_supabaseUrl}/rest/v1/profiles?limit=1");
                    
                    bool success = response.IsSuccessStatusCode;
                    
                    Logger.Log(
                        success ? LogLevel.Info : LogLevel.Warning, 
                        $"User service check - Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                    
                    _userServiceAvailable = success;
                    _lastUserCheck = DateTime.Now;
                    
                    return _userServiceAvailable;
                }
            }
            catch (Exception ex)
            {
                _userServiceAvailable = false;
                _lastUserCheck = DateTime.Now;
                Logger.Log(LogLevel.Error, $"User service availability check failed: {ex.Message}");
                return false;
            }
        }

        // Utility method to ping the Supabase endpoints, can be helpful for initial connection testing
        public static async Task<bool> PingSupabaseAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("apikey", _supabaseKey);
                    
                    var sw = Stopwatch.StartNew();
                    var response = await client.GetAsync(_supabaseUrl);
                    sw.Stop();
                    
                    Logger.Log(LogLevel.Info, 
                        $"Supabase ping: {response.StatusCode}, Time: {sw.ElapsedMilliseconds}ms");
                    
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Supabase ping failed: {ex.Message}");
                return false;
            }
        }
    }
} 