using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;

public interface IBorrowRecordService
{
    Task<IEnumerable<BorrowRecordDTO>> GetAllAsync();
    Task<IEnumerable<BorrowRecordDTO>> GetBorrowRecordsByUserIdAsync(string userId);
    Task<BorrowRecordDTO?> GetByIdAsync(int id);
    Task<BorrowRecordDTO> BorrowBookAsync(string userId, int bookId);
    Task<BorrowRecordDTO> ReturnBookAsync(int borrowRecordId);
}