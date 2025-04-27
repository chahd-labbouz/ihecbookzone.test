using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IHECBookzone.Desktop.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _id;
        private string _email;
        private string _fullName;
        private string _phoneNumber;
        private string _levelOfStudy;
        private string _fieldOfStudy;
        private string _avatarUrl;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private bool _isAdmin;

        public string Id 
        { 
            get => _id; 
            set => SetProperty(ref _id, value); 
        }

        public string Email 
        { 
            get => _email; 
            set => SetProperty(ref _email, value); 
        }

        public string FullName 
        { 
            get => _fullName; 
            set => SetProperty(ref _fullName, value); 
        }

        public string PhoneNumber 
        { 
            get => _phoneNumber; 
            set => SetProperty(ref _phoneNumber, value); 
        }

        public string LevelOfStudy 
        { 
            get => _levelOfStudy; 
            set => SetProperty(ref _levelOfStudy, value); 
        }

        public string FieldOfStudy 
        { 
            get => _fieldOfStudy; 
            set => SetProperty(ref _fieldOfStudy, value); 
        }

        public string AvatarUrl 
        { 
            get => _avatarUrl; 
            set => SetProperty(ref _avatarUrl, value); 
        }

        public DateTime CreatedAt 
        { 
            get => _createdAt; 
            set => SetProperty(ref _createdAt, value); 
        }

        public DateTime UpdatedAt 
        { 
            get => _updatedAt; 
            set => SetProperty(ref _updatedAt, value); 
        }

        public bool IsAdmin 
        { 
            get => _isAdmin; 
            set => SetProperty(ref _isAdmin, value); 
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
} 