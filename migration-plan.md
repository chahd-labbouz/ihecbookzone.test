# IHEC Bookzone Connect - Migration Plan

## Overview
This document outlines the plan to migrate the IHEC Bookzone Connect web application to a .NET WPF desktop application.

## Technology Stack
- **Framework**: WPF (Windows Presentation Foundation)
- **Language**: C#
- **Database**: SQL Server (or keep using Supabase via REST API)
- **Architecture**: MVVM (Model-View-ViewModel)
- **UI Components**: WPF Controls with Material Design or MahApps.Metro

## Project Structure

```
IHECBookzone.Desktop/
├── App.xaml
├── MainWindow.xaml
├── Models/               # Data models (similar to DB types)
├── ViewModels/           # Logic layer
├── Views/                # User interface
│   ├── Pages/            # Main application pages
│   ├── Controls/         # Reusable UI components
│   └── Windows/          # Dialog windows
├── Services/             # Business logic and external services
│   ├── AuthService.cs    # Authentication
│   ├── BookService.cs    # Book-related operations
│   └── ApiService.cs     # Communication with backend
├── Helpers/              # Utility classes
└── Resources/            # Styles, images, etc.
```

## Migration Steps

### 1. Set Up Project Structure
- Create a new WPF application
- Set up MVVM framework (can use MVVM Light or similar)
- Create folder structure

### 2. Design Database Access Layer
- Option 1: Set up Entity Framework to connect to SQL Server
- Option 2: Create API services to connect to existing Supabase backend

### 3. Implement Authentication
- Create login/registration system similar to the current implementation
- Adapt the existing auth flow to use .NET mechanisms

### 4. Create Core Models
Migrate existing data structures to C# classes:
- Book
- User/Profile
- Borrowing
- Reservation
- Notification

### 5. Implement ViewModels
Create ViewModels for each major feature:
- HomeViewModel
- LibraryViewModel
- ProfileViewModel
- AdminDashboardViewModel

### 6. Design UI
- Create main application shell with navigation similar to web app
- Implement pages for each major feature:
  - RoleSelection
  - Login/Register
  - Home dashboard
  - Library catalog
  - Book details
  - User profile
  - Admin dashboard

### 7. Implement Business Logic
- Book browsing/searching
- Borrowing/reservation system
- Admin features
- User management

### 8. Add Styling
- Create custom theme matching IHEC branding
- Implement responsive layouts for different window sizes

### 9. Implement Offline Functionality
- Add local caching for improved performance
- Support offline browsing of previously loaded data

### 10. Testing
- Unit tests for ViewModels
- Integration tests for Services
- UI tests for critical flows

## Feature Comparison

| Web Feature | Desktop Implementation |
|-------------|------------------------|
| Routing | Navigation service with page switching |
| React Context | Application-level services |
| REST API calls | Service layer with HttpClient |
| Responsive design | Grid-based dynamic layouts |
| Tailwind styling | XAML styles and themes |
| User authentication | Custom auth service with token storage |
| Book cards | Custom WPF UserControls |
| Admin dashboard | Admin-specific views |

## UI Components Mapping

| Web Component | WPF Equivalent |
|---------------|----------------|
| Layout | Main application window with ContentControl |
| Navbar | Menu/CommandBar at top of application |
| BookCard | Custom UserControl |
| Button | WPF Button with custom styling |
| Input | TextBox with styling |
| Select | ComboBox |
| Tabs | TabControl |
| Toast notifications | Custom notification system |
| Modal dialogs | WPF dialogs |

## Timeline Estimation
- Project setup and architecture: 1 week
- Core models and data access: 1 week
- Authentication system: 1 week
- Basic UI implementation: 2 weeks
- Business logic implementation: 2 weeks
- Styling and UI refinements: 1 week
- Testing and bug fixes: 1 week
- Deployment preparation: 3 days

Total estimated time: ~9-10 weeks 