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
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

        if (subscription == null || subscription.EndDate < DateTime.Now){
            throw new Exception("User does not have a valid subscription.");
        }

        var activeBorrowsCount = await _context.BorrowRecords
            .CountAsync(b => b.UserId == userId && b.ReturnDate == null);

        if (activeBorrowsCount >= subscription.MaxBorrowLimit){
            throw new Exception("User has reached the maximum number of active borrows.");
        }

        var alreadyBorrowed = await _context.BorrowRecords
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId && b.ReturnDate == null);

        if (alreadyBorrowed != null){
            throw new Exception("You have already borrowed this book.");
        }



        var borrowRecord = new BorrowRecord
        {
            UserId = userId,
            BookId = bookId,
            BorrowDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(subscription.BorrowDurationDays)
        };

        _context.BorrowRecords.Add(borrowRecord);
        await _context.SaveChangesAsync();

        return borrowRecord;
    }

    public async Task ReturnBookAsync(int borrowRecordId)
    {
        var record = await _context.BorrowRecords.FindAsync(borrowRecordId);
        if (record == null) throw new Exception("Borrow record not found.");

        if (record.ReturnDate != null){
            throw new Exception("Book has already been returned.");
        }

        record.ReturnDate = DateTime.Now;
        _context.Entry(record).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByUserIdAsync(string userId)
    {
        return await _context.BorrowRecords
                     .Where(r => r.UserId == userId)
                     .ToListAsync();
    }
}