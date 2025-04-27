using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Utils;

namespace IHECBookzone.Desktop.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _userId; // Current user ID for filtering borrowings
        private readonly IBookService _bookService; // For book availability updates and retrieving book details
        private readonly IAuthService _authService; // For authentication token

        public BorrowingService(IBookService bookService = null, IAuthService authService = null)
        {
            _authService = authService;
            _httpClient = ApiClient.GetClient();
            _baseUrl = ApiSettings.ApiUrl + "/borrowings";
            
            // Get current user ID if auth service is available
            if (_authService != null && _authService.IsAuthenticated)
            {
                _userId = _authService.CurrentUser?.Id;
            }
            
            _bookService = bookService;
            
            // Set auth token if available
            if (_authService != null && !string.IsNullOrEmpty(_authService.SessionToken))
            {
                ApiClient.SetAuthToken(_authService.SessionToken);
            }
        }

        public async Task<List<Borrowing>> GetAllBorrowingsAsync()
        {
            try
            {
                var url = _baseUrl;
                if (!string.IsNullOrEmpty(_userId))
                {
                    url += $"?user_id={_userId}";
                }
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var borrowings = await response.Content.ReadFromJsonAsync<List<Borrowing>>();
                return borrowings ?? new List<Borrowing>();
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to fetch all borrowings");
                throw;
            }
        }

        public async Task<List<Borrowing>> GetActiveBorrowingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/active{(_userId != null ? $"?user_id={_userId}" : "")}");
                response.EnsureSuccessStatusCode();
                
                var borrowings = await response.Content.ReadFromJsonAsync<List<Borrowing>>();
                return borrowings ?? new List<Borrowing>();
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to fetch active borrowings");
                throw;
            }
        }

        public async Task<List<Borrowing>> GetOverdueBorrowingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/overdue{(_userId != null ? $"?user_id={_userId}" : "")}");
                response.EnsureSuccessStatusCode();
                
                var borrowings = await response.Content.ReadFromJsonAsync<List<Borrowing>>();
                return borrowings ?? new List<Borrowing>();
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to fetch overdue borrowings");
                throw;
            }
        }

        public async Task<List<Borrowing>> GetUserBorrowingsAsync(bool onlyActive = false)
        {
            try
            {
                if (string.IsNullOrEmpty(_userId))
                {
                    throw new InvalidOperationException("User ID is required to fetch user borrowings");
                }

                var borrowings = await GetAllBorrowingsAsync();
                
                if (onlyActive)
                {
                    borrowings = borrowings.Where(b => b.ReturnDate == null).ToList();
                }
                
                return borrowings;
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to fetch user borrowings");
                throw;
            }
        }

        public async Task<List<Borrowing>> GetUserBorrowingsWithDetailsAsync(string userId, bool includeReturned = true)
        {
            try
            {
                if (_bookService == null)
                {
                    throw new InvalidOperationException("BookService is required to fetch borrowing details");
                }
                
                var queryParams = new List<string>();
                queryParams.Add("select=*");
                queryParams.Add($"user_id=eq.{userId}");

                if (!includeReturned)
                {
                    queryParams.Add("return_date=is.null");
                }

                var query = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"{_baseUrl}?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var borrowings = await response.Content.ReadFromJsonAsync<List<BorrowingDto>>();
                    var result = new List<Borrowing>();

                    if (borrowings != null)
                    {
                        foreach (var borrowing in borrowings)
                        {
                            var record = MapToBorrowing(borrowing);
                            // Get book details
                            var book = await _bookService.GetBookByIdAsync(borrowing.BookId);
                            if (book != null)
                            {
                                record.Book = book;
                            }
                            result.Add(record);
                        }
                    }

                    return result;
                }

                return new List<Borrowing>();
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to fetch user borrowings with details");
                return new List<Borrowing>();
            }
        }

        public async Task<bool> BorrowBookAsync(string bookId, string userId)
        {
            try
            {
                var borrowDate = DateTime.Now;
                var dueDate = borrowDate.AddDays(14); // 2-week period

                var borrowRequest = new
                {
                    book_id = bookId,
                    user_id = userId,
                    borrow_date = borrowDate.ToString("yyyy-MM-dd"),
                    due_date = dueDate.ToString("yyyy-MM-dd"),
                    status = "borrowed"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(borrowRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);

                if (response.IsSuccessStatusCode && _bookService != null)
                {
                    // Update available copies if BookService is available
                    await _bookService.UpdateBookAvailability(bookId, -1);
                    return true;
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to borrow book");
                return false;
            }
        }

        public async Task<bool> ReturnBookAsync(string borrowingId)
        {
            try
            {
                // First get the borrowing record to know which book to update
                var response = await _httpClient.GetAsync($"{_baseUrl}?id=eq.{borrowingId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var borrowings = await response.Content.ReadFromJsonAsync<List<BorrowingDto>>();
                
                if (borrowings == null || borrowings.Count == 0)
                {
                    return false;
                }

                var borrowing = borrowings[0];
                
                // Update the borrowing record with return date and status
                var returnDate = DateTime.Now;
                var updateRequest = new
                {
                    return_date = returnDate.ToString("yyyy-MM-dd"),
                    status = "returned"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(updateRequest),
                    Encoding.UTF8,
                    "application/json");

                var updateResponse = await _httpClient.PatchAsync($"{_baseUrl}?id=eq.{borrowingId}", content);
                
                if (updateResponse.IsSuccessStatusCode && _bookService != null)
                {
                    // Update available copies if BookService is available
                    await _bookService.UpdateBookAvailability(borrowing.BookId, 1);
                    return true;
                }

                return updateResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Failed to return book with borrowing ID {borrowingId}");
                return false;
            }
        }

        public async Task<bool> ReserveBookAsync(string bookId, string userId)
        {
            try
            {
                var reservationRequest = new
                {
                    book_id = bookId,
                    user_id = userId,
                    status = "pending"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(reservationRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("/rest/v1/reservations", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to reserve book");
                return false;
            }
        }

        public async Task<bool> DeleteBorrowingAsync(string borrowingId)
        {
            try
            {
                // First get the borrowing record to know which book to update
                var response = await _httpClient.GetAsync($"{_baseUrl}?id=eq.{borrowingId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var borrowings = await response.Content.ReadFromJsonAsync<List<BorrowingDto>>();
                
                if (borrowings == null || borrowings.Count == 0)
                {
                    return false;
                }

                var borrowing = borrowings[0];
                
                // Check if the book was returned
                if (borrowing.ReturnDate == null && _bookService != null)
                {
                    // If not returned, we need to update the book availability
                    await _bookService.UpdateBookAvailability(borrowing.BookId, 1);
                }

                // Delete the borrowing record
                var deleteResponse = await _httpClient.DeleteAsync($"{_baseUrl}?id=eq.{borrowingId}");
                return deleteResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Failed to delete borrowing with ID {borrowingId}");
                return false;
            }
        }

        private Borrowing MapToBorrowing(BorrowingDto dto)
        {
            var borrowing = new Borrowing
            {
                Id = dto.Id,
                UserId = dto.UserId,
                BookId = dto.BookId,
                BorrowDate = dto.BorrowDate,
                DueDate = dto.DueDate,
                ReturnDate = dto.ReturnDate,
                Status = dto.Status
            };
            
            // DueStatus is a read-only computed property, no need to set it
            
            return borrowing;
        }

        private void LogError(Exception ex, string message)
        {
            // In a real application, implement proper logging
            Console.WriteLine($"{message}: {ex.Message}");
        }

        // DTO for borrowings
        private class BorrowingDto
        {
            public string Id { get; set; }
            public string UserId { get; set; }
            public string BookId { get; set; }
            public DateTime BorrowDate { get; set; }
            public DateTime DueDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public string Status { get; set; }
        }
    }
} 