using System.Collections.Generic;
using System.Threading.Tasks;
using IHECBookzone.Desktop.Models;

namespace IHECBookzone.Desktop.Services
{
    public interface IBookService
    {
        void SetAuthToken(string token);
        Task<List<Book>> GetBooksAsync(string searchTerm = null, string category = null, string academicLevel = null, bool? available = null);
        Task<Book> GetBookByIdAsync(string id);
        Task<List<string>> GetCategoriesAsync();
        Task<bool> UpdateBookAvailability(string bookId, int change);
        Task<bool> DeleteBookAsync(string bookId);
    }
} 