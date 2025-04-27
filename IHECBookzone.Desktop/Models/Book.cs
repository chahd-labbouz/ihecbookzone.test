using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace IHECBookzone.Desktop.Models
{
    public class Book : INotifyPropertyChanged
    {
        private string _id;
        private string _title;
        private string _author;
        private string _coverImageUrl;
        private string _category;
        private string _academicLevel;
        private string _isbn;
        private string _publisher;
        private int _publicationYear;
        private string _description;
        private int _totalCopies;
        private int _availableCopies;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private Brush _backgroundColor;
        private string _language;

        public string Id 
        { 
            get => _id; 
            set => SetProperty(ref _id, value); 
        }

        public string Title 
        { 
            get => _title; 
            set => SetProperty(ref _title, value); 
        }

        public string Author 
        { 
            get => _author; 
            set => SetProperty(ref _author, value); 
        }

        public string CoverImageUrl 
        { 
            get => _coverImageUrl; 
            set => SetProperty(ref _coverImageUrl, value); 
        }

        public string Category 
        { 
            get => _category; 
            set => SetProperty(ref _category, value); 
        }

        public string AcademicLevel 
        { 
            get => _academicLevel; 
            set => SetProperty(ref _academicLevel, value); 
        }

        public string ISBN 
        { 
            get => _isbn; 
            set => SetProperty(ref _isbn, value); 
        }

        public string Publisher 
        { 
            get => _publisher; 
            set => SetProperty(ref _publisher, value); 
        }

        public int PublicationYear 
        { 
            get => _publicationYear; 
            set => SetProperty(ref _publicationYear, value); 
        }

        public string Description 
        { 
            get => _description; 
            set => SetProperty(ref _description, value); 
        }

        public int TotalCopies 
        { 
            get => _totalCopies; 
            set => SetProperty(ref _totalCopies, value); 
        }

        public int AvailableCopies 
        { 
            get => _availableCopies; 
            set => SetProperty(ref _availableCopies, value); 
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
        
        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        public string Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

        public bool IsAvailable => AvailableCopies > 0;

        // Return formatted values for display
        public string FormattedPublicationYear => PublicationYear.ToString();
        public string FormattedAvailability => IsAvailable ? "Available" : "Not Available";
        public string FormattedCopies => $"{AvailableCopies}/{TotalCopies}";

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