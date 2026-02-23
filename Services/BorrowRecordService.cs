using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class BorrowRecordService : IBorrowRecordService
{
    private readonly LibraryContext _context;

    public BorrowRecordService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BorrowRecord>> GetAllAsync()
    {
        return await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .ToListAsync();
    }

    public async Task<BorrowRecord?> GetByIdAsync(int id)
    {
        return await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.BorrowRecordId == id);
    }

    public async Task<BorrowRecord> BorrowBookAsync(string userId, int bookId)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.Subscription == null || user.Subscription.EndDate < DateTime.Now)
            throw new Exception("User does not have a valid subscription.");

        var borrowRecord = new BorrowRecord
        {
            UserId = userId,
            BookId = bookId,
            BorrowDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(user.Subscription.BorrowDurationDays)
        };

        _context.BorrowRecords.Add(borrowRecord);
        await _context.SaveChangesAsync();

        return borrowRecord;
    }

    public async Task ReturnBookAsync(int borrowRecordId)
    {
        var record = await _context.BorrowRecords.FindAsync(borrowRecordId);
        if (record == null) throw new Exception("Borrow record not found.");

        record.ReturnDate = DateTime.Now;
        _context.Entry(record).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}