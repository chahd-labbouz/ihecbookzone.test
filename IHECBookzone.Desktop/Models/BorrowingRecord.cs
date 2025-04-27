using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IHECBookzone.Desktop.Models
{
    public class Borrowing : INotifyPropertyChanged
    {
        private string _id;
        private string _userId;
        private string _bookId;
        private Book _book;
        private DateTime _borrowDate;
        private DateTime _dueDate;
        private DateTime? _returnDate;
        private string _status; // "borrowed", "returned", "overdue"

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        public string BookId
        {
            get => _bookId;
            set => SetProperty(ref _bookId, value);
        }

        public Book Book
        {
            get => _book;
            set => SetProperty(ref _book, value);
        }

        public DateTime BorrowDate
        {
            get => _borrowDate;
            set => SetProperty(ref _borrowDate, value);
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        public DateTime? ReturnDate
        {
            get => _returnDate;
            set => SetProperty(ref _returnDate, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        // Computed properties
        public bool IsReturned => ReturnDate.HasValue;

        public bool IsOverdue => !IsReturned && DateTime.Now > DueDate;

        public int DaysRemaining
        {
            get
            {
                if (IsReturned) return 0;
                
                var daysRemaining = (DueDate - DateTime.Now).Days;
                return daysRemaining > 0 ? daysRemaining : 0;
            }
        }

        public int DaysOverdue
        {
            get
            {
                if (IsReturned || !IsOverdue) return 0;
                
                return (DateTime.Now - DueDate).Days;
            }
        }

        public string DueStatus
        {
            get
            {
                if (IsReturned)
                    return "Returned";
                else if (IsOverdue)
                    return "Overdue";
                else
                    return "Active";
            }
        }

        // Method to calculate due status
        public string CalculateDueStatus()
        {
            if (IsReturned)
                return "Returned";
            else if (IsOverdue)
                return "Overdue";
            else
                return "Active";
        }

        // Constructors
        public Borrowing()
        {
            Book = new Book();
        }
        
        public Borrowing(string id, Book book, DateTime borrowDate, DateTime dueDate, DateTime? returnDate = null)
        {
            Id = id;
            Book = book;
            BookId = book?.Id;
            BorrowDate = borrowDate;
            DueDate = dueDate;
            ReturnDate = returnDate;
            
            // Set status based on dates
            if (ReturnDate.HasValue)
            {
                Status = "returned";
            }
            else if (DateTime.Now > DueDate)
            {
                Status = "overdue";
            }
            else
            {
                Status = "borrowed";
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
} 