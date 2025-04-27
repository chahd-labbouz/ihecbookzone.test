using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IHECBookzone.Desktop.Commands;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Services;

namespace IHECBookzone.Desktop.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private User _currentUser;
        private User _editableUser;
        private bool _isEditing;
        private bool _isSaving;
        private Dictionary<string, string> _validationErrors;
        
        public User CurrentUser
        {
            get => _currentUser;
            private set => SetProperty(ref _currentUser, value);
        }
        
        public User EditableUser
        {
            get => _editableUser;
            private set => SetProperty(ref _editableUser, value);
        }
        
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }
        
        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }
        
        public Dictionary<string, string> ValidationErrors
        {
            get => _validationErrors;
            private set => SetProperty(ref _validationErrors, value);
        }

        public ICommand EditProfileCommand { get; }
        public ICommand SaveProfileCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        
        public ProfileViewModel(AuthService authService, UserService userService = null)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userService = userService ?? new UserService(_authService); // Pass AuthService to UserService
            _validationErrors = new Dictionary<string, string>();
            
            // Get current user from auth service
            CurrentUser = _authService.CurrentUser;
            
            // Set the current user in UserService as well
            if (_userService != null && CurrentUser != null)
            {
                _userService.CurrentUser = CurrentUser;
            }
            
            // Create a copy of the user for editing
            CreateEditableCopy();
            
            // Initialize commands
            EditProfileCommand = new RelayCommand(_ => StartEditing());
            SaveProfileCommand = new RelayCommand(async _ => await SaveProfileAsync(), _ => CanSaveProfile());
            CancelEditCommand = new RelayCommand(_ => CancelEdit());
            ChangePasswordCommand = new RelayCommand(_ => ShowChangePasswordDialog());
        }
        
        private void CreateEditableCopy()
        {
            if (CurrentUser == null) return;
            
            EditableUser = new User
            {
                Id = CurrentUser.Id,
                Email = CurrentUser.Email,
                FullName = CurrentUser.FullName,
                PhoneNumber = CurrentUser.PhoneNumber,
                LevelOfStudy = CurrentUser.LevelOfStudy,
                FieldOfStudy = CurrentUser.FieldOfStudy,
                AvatarUrl = CurrentUser.AvatarUrl,
                CreatedAt = CurrentUser.CreatedAt,
                UpdatedAt = CurrentUser.UpdatedAt,
                IsAdmin = CurrentUser.IsAdmin
            };
        }
        
        private void StartEditing()
        {
            // Reset validation errors
            ValidationErrors.Clear();
            OnPropertyChanged(nameof(ValidationErrors));
            
            // Enter edit mode
            IsEditing = true;
        }
        
        private async Task SaveProfileAsync()
        {
            if (EditableUser == null) return;
            
            // Validate before saving
            if (!ValidateProfile()) return;
            
            try
            {
                IsSaving = true;
                
                // Save changes using UserService
                bool success = await _userService.UpdateProfileAsync(EditableUser);
                
                if (success)
                {
                    // Update current user with new values
                    CurrentUser.FullName = EditableUser.FullName;
                    CurrentUser.PhoneNumber = EditableUser.PhoneNumber;
                    CurrentUser.LevelOfStudy = EditableUser.LevelOfStudy;
                    CurrentUser.FieldOfStudy = EditableUser.FieldOfStudy;
                    CurrentUser.UpdatedAt = DateTime.Now;
                    
                    // Exit edit mode
                    IsEditing = false;
                    
                    MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update profile. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }
        
        private void CancelEdit()
        {
            // Revert changes by creating a new editable copy from the current user
            CreateEditableCopy();
            
            // Clear validation errors
            ValidationErrors.Clear();
            OnPropertyChanged(nameof(ValidationErrors));
            
            // Exit edit mode
            IsEditing = false;
        }
        
        private bool ValidateProfile()
        {
            ValidationErrors.Clear();
            
            // Validate full name
            if (string.IsNullOrWhiteSpace(EditableUser.FullName))
            {
                ValidationErrors["FullName"] = "Full name is required.";
            }
            else if (EditableUser.FullName.Length < 3)
            {
                ValidationErrors["FullName"] = "Full name must be at least 3 characters.";
            }
            
            // Validate phone number (simple validation)
            if (string.IsNullOrWhiteSpace(EditableUser.PhoneNumber))
            {
                ValidationErrors["PhoneNumber"] = "Phone number is required.";
            }
            else if (!EditableUser.PhoneNumber.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' '))
            {
                ValidationErrors["PhoneNumber"] = "Phone number can only contain digits, +, -, and spaces.";
            }
            
            // Validate level of study
            if (string.IsNullOrWhiteSpace(EditableUser.LevelOfStudy))
            {
                ValidationErrors["LevelOfStudy"] = "Level of study is required.";
            }
            
            // Validate field of study
            if (string.IsNullOrWhiteSpace(EditableUser.FieldOfStudy))
            {
                ValidationErrors["FieldOfStudy"] = "Field of study is required.";
            }
            
            // Notify property changed to update UI
            OnPropertyChanged(nameof(ValidationErrors));
            
            return ValidationErrors.Count == 0;
        }
        
        private bool CanSaveProfile()
        {
            return IsEditing && !IsSaving && EditableUser != null;
        }
        
        private void ShowChangePasswordDialog()
        {
            // Create a simple password change dialog
            var dialog = new PasswordChangeDialog();
            if (dialog.ShowDialog() == true)
            {
                ChangePasswordAsync(dialog.CurrentPassword, dialog.NewPassword);
            }
        }
        
        private async void ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Current password and new password are required.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            try
            {
                bool success = await _userService.ChangePasswordAsync(
                    CurrentUser.Id, currentPassword, newPassword);
                
                if (success)
                {
                    MessageBox.Show("Password changed successfully.", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to change password. Please check your current password and try again.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
    
    // Simple dialog for password change
    public class PasswordChangeDialog : Window
    {
        public string CurrentPassword { get; private set; }
        public string NewPassword { get; private set; }
        
        public PasswordChangeDialog()
        {
            Title = "Change Password";
            Width = 350;
            Height = 200;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            var title = new TextBlock
            {
                Text = "Change Password",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(title, 0);
            
            var currentPasswordLabel = new TextBlock
            {
                Text = "Current Password:",
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(currentPasswordLabel, 1);
            
            var currentPasswordBox = new PasswordBox
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(currentPasswordBox, 2);
            
            var newPasswordLabel = new TextBlock
            {
                Text = "New Password:",
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(newPasswordLabel, 3);
            
            var newPasswordBox = new PasswordBox
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(newPasswordBox, 4);
            
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetRow(buttonPanel, 5);
            
            var cancelButton = new Button
            {
                Content = "Cancel",
                Padding = new Thickness(15, 5, 15, 5),
                Margin = new Thickness(0, 0, 10, 0)
            };
            cancelButton.Click += (sender, e) => 
            {
                DialogResult = false;
                Close();
            };
            
            var saveButton = new Button
            {
                Content = "Save",
                Padding = new Thickness(15, 5, 15, 5),
                Background = new SolidColorBrush(Color.FromRgb(59, 80, 178)),
                Foreground = Brushes.White
            };
            saveButton.Click += (sender, e) => 
            {
                if (string.IsNullOrEmpty(currentPasswordBox.Password) || string.IsNullOrEmpty(newPasswordBox.Password))
                {
                    MessageBox.Show("Please enter both current and new passwords.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                CurrentPassword = currentPasswordBox.Password;
                NewPassword = newPasswordBox.Password;
                DialogResult = true;
                Close();
            };
            
            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(saveButton);
            
            grid.Children.Add(title);
            grid.Children.Add(currentPasswordLabel);
            grid.Children.Add(currentPasswordBox);
            grid.Children.Add(newPasswordLabel);
            grid.Children.Add(newPasswordBox);
            grid.Children.Add(buttonPanel);
            
            Content = grid;
        }
    }
} 