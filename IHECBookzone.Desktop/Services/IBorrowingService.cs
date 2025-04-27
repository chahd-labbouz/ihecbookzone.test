using System.Collections.Generic;
using System.Threading.Tasks;
using IHECBookzone.Desktop.Models;

namespace IHECBookzone.Desktop.Services
{
    public interface IBorrowingService
    {
        Task<List<Borrowing>> GetAllBorrowingsAsync();
        Task<List<Borrowing>> GetActiveBorrowingsAsync();
        Task<List<Borrowing>> GetOverdueBorrowingsAsync();
        Task<List<Borrowing>> GetUserBorrowingsAsync(bool onlyActive = false);
        Task<List<Borrowing>> GetUserBorrowingsWithDetailsAsync(string userId, bool includeReturned = true);
        Task<bool> BorrowBookAsync(string bookId, string userId);
        Task<bool> ReturnBookAsync(string borrowingId);
        Task<bool> ReserveBookAsync(string bookId, string userId);
        Task<bool> DeleteBorrowingAsync(string borrowingId);
    }
} 