using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IHECBookzone.Desktop.Models;
using IHECBookzone.Desktop.Utils;

namespace IHECBookzone.Desktop.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl = "https://libbpevezajvedxiebvv.supabase.co";
        private readonly string _supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxpYmJwZXZlemFqdmVkeGllYnZ2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDU1ODQyODgsImV4cCI6MjA2MTE2MDI4OH0.SdRKdN_jt-8GPiGUMZug_AAFtFEiAIJh4vGegqqZ3is";

        public BookService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.BaseAddress = new Uri(_supabaseUrl);
            
            // Increase timeout to handle potential network delays
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            // Initialize service availability check
            _ = CheckServiceAvailabilityAsync();
        }
        
        private async Task CheckServiceAvailabilityAsync()
        {
            // Check book service availability on initialization
            await ServiceAvailabilityChecker.CheckBookServiceAvailabilityAsync();
        }

        public void SetAuthToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
        }

        public async Task<List<Book>> GetBooksAsync(string searchTerm = null, string category = null, string academicLevel = null, bool? available = null)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckBookServiceAvailabilityAsync())
                {
                    throw new Exception("Book service is not available.");
                }
                
                var queryParams = new List<string>();
                queryParams.Add("select=*");

                // Add filter conditions
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    queryParams.Add($"or=(title.ilike.%{searchTerm}%,author.ilike.%{searchTerm}%)");
                }

                if (!string.IsNullOrEmpty(category) && category != "All Categories")
                {
                    queryParams.Add($"category=eq.{category}");
                }

                if (!string.IsNullOrEmpty(academicLevel) && academicLevel != "all")
                {
                    queryParams.Add($"academic_level=eq.{academicLevel}");
                }

                if (available.HasValue)
                {
                    if (available.Value)
                    {
                        queryParams.Add("available_copies=gt.0");
                    }
                    else
                    {
                        queryParams.Add("available_copies=eq.0");
                    }
                }

                var query = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"/rest/v1/books?{query}");

                if (response.IsSuccessStatusCode)
                {
                    var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
                    return books?.ConvertAll(MapToBook) ?? new List<Book>();
                }

                return new List<Book>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                throw new Exception("Book service is not available.", ex);
            }
        }

        public async Task<Book> GetBookByIdAsync(string id)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckBookServiceAvailabilityAsync())
                {
                    throw new Exception("Book service is not available.");
                }
                
                var response = await _httpClient.GetAsync($"/rest/v1/books?id=eq.{id}&select=*");

                if (response.IsSuccessStatusCode)
                {
                    var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
                    if (books != null && books.Count > 0)
                    {
                        return MapToBook(books[0]);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching book by ID: {ex.Message}");
                throw new Exception("Book service is not available.", ex);
            }
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckBookServiceAvailabilityAsync())
                {
                    throw new Exception("Book service is not available.");
                }
                
                var response = await _httpClient.GetAsync("/rest/v1/books?select=category");

                if (response.IsSuccessStatusCode)
                {
                    var books = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
                    var categories = new HashSet<string>();
                    
                    foreach (var book in books)
                    {
                        if (!string.IsNullOrEmpty(book.Category))
                        {
                            categories.Add(book.Category);
                        }
                    }

                    var result = new List<string>(categories);
                    result.Sort();
                    result.Insert(0, "All Categories");
                    return result;
                }

                return new List<string> { "All Categories" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                throw new Exception("Book service is not available.", ex);
            }
        }

        // Method to update book availability, now made public for BorrowingService
        public async Task<bool> UpdateBookAvailability(string bookId, int change)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckBookServiceAvailabilityAsync())
                {
                    throw new Exception("Book service is not available.");
                }
                
                // First get the current book
                var book = await GetBookByIdAsync(bookId);
                if (book == null) return false;

                var newAvailableCopies = book.AvailableCopies + change;
                
                // Ensure we don't go below 0 or above total
                newAvailableCopies = Math.Max(0, Math.Min(newAvailableCopies, book.TotalCopies));

                var updateRequest = new
                {
                    available_copies = newAvailableCopies
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(updateRequest),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PatchAsync($"/rest/v1/books?id=eq.{bookId}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book availability: {ex.Message}");
                throw new Exception("Book service is not available.", ex);
            }
        }

        public async Task<bool> DeleteBookAsync(string bookId)
        {
            try
            {
                // Check service availability before making the request
                if (!await ServiceAvailabilityChecker.CheckBookServiceAvailabilityAsync())
                {
                    throw new Exception("Book service is not available.");
                }
                
                var response = await _httpClient.DeleteAsync($"/rest/v1/books?id=eq.{bookId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                throw new Exception("Book service is not available.", ex);
            }
        }

        private Book MapToBook(BookDto dto)
        {
            return new Book
            {
                Id = dto.Id,
                Title = dto.Title,
                Author = dto.Author,
                CoverImageUrl = dto.CoverImageUrl,
                Category = dto.Category,
                AcademicLevel = dto.AcademicLevel,
                ISBN = dto.ISBN,
                Publisher = dto.Publisher,
                PublicationYear = dto.PublicationYear,
                Description = dto.Description,
                TotalCopies = dto.TotalCopies,
                AvailableCopies = dto.AvailableCopies,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        // DTOs for parsing JSON responses
        private class BookDto
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string CoverImageUrl { get; set; }
            public string Category { get; set; }
            public string AcademicLevel { get; set; }
            public string ISBN { get; set; }
            public string Publisher { get; set; }
            public int PublicationYear { get; set; }
            public string Description { get; set; }
            public int TotalCopies { get; set; }
            public int AvailableCopies { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        private class CategoryDto
        {
            public string Category { get; set; }
        }
    }
} 