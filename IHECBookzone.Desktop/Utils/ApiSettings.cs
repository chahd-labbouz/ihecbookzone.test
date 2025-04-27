using System;

namespace IHECBookzone.Desktop.Utils
{
    public static class ApiSettings
    {
        // Supabase configuration
        public static string SupabaseUrl { get; } = "https://libbpevezajvedxiebvv.supabase.co";
        public static string SupabaseKey { get; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxpYmJwZXZlemFqdmVkeGllYnZ2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDU1ODQyODgsImV4cCI6MjA2MTE2MDI4OH0.SdRKdN_jt-8GPiGUMZug_AAFtFEiAIJh4vGegqqZ3is";
        
        // REST API endpoints
        public static string ApiUrl => $"{SupabaseUrl}/rest/v1";
        public static string AuthUrl => $"{SupabaseUrl}/auth/v1";
        
        // Default timeout in seconds
        public static int DefaultTimeout { get; } = 30;
        
        // Table names
        public static string BooksTable { get; } = "books";
        public static string BorrowingsTable { get; } = "borrowings";
        public static string ProfilesTable { get; } = "profiles";
        public static string ReservationsTable { get; } = "reservations";
        public static string NotificationsTable { get; } = "notifications";

        // Common HTTP headers
        public static (string Key, string Value) ApiKeyHeader => ("apikey", SupabaseKey);
        public static (string Key, string Value) PreferHeader => ("Prefer", "return=representation");
        public static string AuthHeaderValue(string token) => $"Bearer {token}";
        
        // Auth endpoints
        public static string SignInEndpoint => $"{AuthUrl}/token?grant_type=password";
        public static string SignUpEndpoint => $"{AuthUrl}/signup";
        public static string ResetPasswordEndpoint => $"{AuthUrl}/recover";
        
        // Helper methods
        public static Uri GetEndpointUri(string relativePath) => new Uri($"{ApiUrl}/{relativePath}");
        public static Uri GetAuthEndpointUri(string relativePath) => new Uri($"{AuthUrl}/{relativePath}");
        
        // Query parameters
        public static string BuildQueryString(params string[] queryParams) => string.Join("&", queryParams);
    }
} 