using System;
using System.Windows.Controls;

namespace IHECBookzone.Desktop.Services
{
    public interface INavigationService
    {
        Frame MainFrame { get; set; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        
        void Initialize(Frame frame);
        bool NavigateTo<T>(object parameter = null) where T : Page;
        bool NavigateTo(Type pageType, object parameter = null);
        bool NavigateTo(string pageName, object parameter = null);
        bool NavigateToUri(Uri uri);
        bool GoBack();
        bool GoForward();
        void ClearHistory();
        void RefreshCurrentPage();
    }
} 