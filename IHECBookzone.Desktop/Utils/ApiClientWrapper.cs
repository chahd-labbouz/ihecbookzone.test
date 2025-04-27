using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IHECBookzone.Desktop.Utils
{
    public interface IApiClient
    {
        HttpClient GetClient();
        void SetAuthToken(string token, int expiresIn = 3600);
        bool IsTokenValid();
        Task<HttpResponseMessage> SendWithAuthAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
        void SubscribeToTokenRefresh(EventHandler<string> handler);
        void UnsubscribeFromTokenRefresh(EventHandler<string> handler);
        void SubscribeToApiErrors(EventHandler<Exception> handler);
        void UnsubscribeFromApiErrors(EventHandler<Exception> handler);
    }

    public class ApiClientWrapper : IApiClient
    {
        public HttpClient GetClient() => ApiClient.GetClient();
        
        public void SetAuthToken(string token, int expiresIn = 3600) => 
            ApiClient.SetAuthToken(token, expiresIn);
        
        public bool IsTokenValid() => ApiClient.IsTokenValid();
        
        public Task<HttpResponseMessage> SendWithAuthAsync(HttpRequestMessage request, CancellationToken cancellationToken = default) => 
            ApiClient.SendWithAuthAsync(request, cancellationToken);
        
        public void SubscribeToTokenRefresh(EventHandler<string> handler) => 
            ApiClient.SubscribeToTokenRefresh(handler);
        
        public void UnsubscribeFromTokenRefresh(EventHandler<string> handler) => 
            ApiClient.UnsubscribeFromTokenRefresh(handler);
        
        public void SubscribeToApiErrors(EventHandler<Exception> handler) => 
            ApiClient.SubscribeToApiErrors(handler);
        
        public void UnsubscribeFromApiErrors(EventHandler<Exception> handler) => 
            ApiClient.UnsubscribeFromApiErrors(handler);
    }
} 