using LibraryManagementSystem.Models;

public interface IBorrowRecordService
{
    Task<IEnumerable<BorrowRecord>> GetAllAsync();
    Task<BorrowRecord?> GetByIdAsync(int id);
    Task<BorrowRecord> BorrowBookAsync(string userId, int bookId);
    Task ReturnBookAsync(int borrowRecordId);

    Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByUserIdAsync(string userId);
}