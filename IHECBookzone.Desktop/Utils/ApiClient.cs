using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace IHECBookzone.Desktop.Utils
{
    public static class ApiClient
    {
        private static HttpClient _httpClient;
        private static string _currentToken;
        private static DateTime _tokenExpiryTime = DateTime.MinValue;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static bool _isRefreshing = false;
        private static event EventHandler<string> TokenRefreshed;
        private static event EventHandler<Exception> ApiError;
        
        static ApiClient()
        {
            InitializeHttpClient();
        }
        
        private static void InitializeHttpClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiSettings.SupabaseUrl);
            _httpClient.DefaultRequestHeaders.Add(ApiSettings.ApiKeyHeader.Key, ApiSettings.ApiKeyHeader.Value);
            _httpClient.Timeout = TimeSpan.FromSeconds(ApiSettings.DefaultTimeout);
        }
        
        public static HttpClient GetClient()
        {
            if (_httpClient == null)
            {
                InitializeHttpClient();
            }
            
            return _httpClient;
        }
        
        public static void SetAuthToken(string token, int expiresIn = 3600)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _currentToken = token;
                    _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expiresIn);
                    
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", ApiSettings.AuthHeaderValue(token));
                    
                    Logger.Log(LogLevel.Info, "Auth token set and will expire at " + _tokenExpiryTime.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Failed to set auth token: {ex.Message}");
                throw;
            }
        }
        
        public static bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(_currentToken) && DateTime.UtcNow < _tokenExpiryTime.AddMinutes(-5);
        }
        
        public static async Task<HttpResponseMessage> SendWithAuthAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check token validity before sending request
                if (!IsTokenValid() && !_isRefreshing)
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        // Double-check token validity after acquiring semaphore
                        if (!IsTokenValid() && !_isRefreshing)
                        {
                            _isRefreshing = true;
                            await RefreshTokenAsync();
                            _isRefreshing = false;
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                
                // Add current auth token to request
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _currentToken);
                
                // Send request
                var response = await _httpClient.SendAsync(request, cancellationToken);
                
                // Handle auth errors
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        if (!_isRefreshing)
                        {
                            _isRefreshing = true;
                            bool success = await RefreshTokenAsync();
                            _isRefreshing = false;
                            
                            if (success)
                            {
                                // Retry with new token
                                request = await CloneRequestAsync(request);
                                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _currentToken);
                                return await _httpClient.SendAsync(request, cancellationToken);
                            }
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
                
                return response;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"API request failed: {ex.Message}");
                ApiError?.Invoke(null, ex);
                throw;
            }
        }
        
        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri);
            
            // Copy content if present
            if (request.Content != null)
            {
                var contentBytes = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(contentBytes);
                
                // Copy content headers
                if (request.Content.Headers != null)
                {
                    foreach (var header in request.Content.Headers)
                    {
                        clone.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }
            
            // Copy request headers except Authorization which will be added later
            foreach (var header in request.Headers.Where(h => h.Key != "Authorization"))
            {
                clone.Headers.Add(header.Key, header.Value);
            }
            
            return clone;
        }
        
        private static async Task<bool> RefreshTokenAsync()
        {
            try
            {
                // Implementation of token refresh logic
                // In Supabase, you would call the refresh token endpoint
                
                Logger.Log(LogLevel.Info, "Token refresh is not currently implemented - will be handled by service layer");
                
                // Notify subscribers that token is refreshed
                if (TokenRefreshed != null && !string.IsNullOrEmpty(_currentToken))
                {
                    TokenRefreshed.Invoke(null, _currentToken);
                }
                
                return !string.IsNullOrEmpty(_currentToken);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Failed to refresh token: {ex.Message}");
                return false;
            }
        }
        
        public static void SubscribeToTokenRefresh(EventHandler<string> handler)
        {
            TokenRefreshed += handler;
        }
        
        public static void UnsubscribeFromTokenRefresh(EventHandler<string> handler)
        {
            TokenRefreshed -= handler;
        }
        
        public static void SubscribeToApiErrors(EventHandler<Exception> handler)
        {
            ApiError += handler;
        }
        
        public static void UnsubscribeFromApiErrors(EventHandler<Exception> handler)
        {
            ApiError -= handler;
        }
        
        // Helper methods for working with JSON requests
        public static StringContent CreateJsonContent<T>(T data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        
        public static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            
            try
            {
                return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                Logger.Log(LogLevel.Error, $"Failed to deserialize response: {ex.Message}. JSON: {jsonString}");
                throw new ApplicationException($"Failed to parse API response: {ex.Message}", ex);
            }
        }
        
        // Helper method to check API availability
        public static async Task<bool> CheckApiAvailabilityAsync()
        {
            return await ServiceAvailabilityChecker.PingSupabaseAsync();
        }
    }
} 