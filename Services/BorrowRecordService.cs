using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class BorrowRecordService : IBorrowRecordService
{
    private readonly LibraryContext _context;
    private readonly ISubscriptionService _subscriptionService;

    public BorrowRecordService(LibraryContext context, ISubscriptionService subscriptionService)
    {
        _context = context;
        _subscriptionService = subscriptionService;
    }

    public async Task<IEnumerable<BorrowRecordDTO>> GetAllAsync()
    {
        var records =await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .ToListAsync();

        return records.Select(MapToDTO);    

    }
    
    public async Task<IEnumerable<BorrowRecordDTO>> GetBorrowRecordsByUserIdAsync(string userId)
    {
        var records = await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return records.Select(MapToDTO);
    }

    public async Task<BorrowRecordDTO?> GetByIdAsync(int id)
    {
        var record = await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.BorrowRecordId == id);

        return record != null ? MapToDTO(record) : null;
    }

    public async Task<BorrowRecordDTO> BorrowBookAsync(string userId, int bookId)
    {
        var isValid = await _subscriptionService.IsSubscriptionValidAsync(userId);
        if (!isValid) 
        {
            throw new Exception("User does not have an active subscription");
        }

        var activeBorrows = await _context.BorrowRecords
            .CountAsync(b => b.UserId == userId && b.ReturnDate == null);

        var subscription = await _subscriptionService.GetByUserIdAsync(userId);
        if (activeBorrows >= subscription.MaxBorrowLimit)
            throw new Exception($"Borrow limit reached. You can only borrow {subscription.MaxBorrowLimit} books."); 

        var alreadyBorrowed = await _context.BorrowRecords
            .AnyAsync(b => b.UserId == userId && b.BookId == bookId && b.ReturnDate == null);
        if (alreadyBorrowed)
        {
            throw new Exception("User has already borrowed this book.");
        }

        var isBorrowed = await _context.BorrowRecords
            .AnyAsync(b => b.BookId == bookId && b.ReturnDate == null);

        if (isBorrowed)
            throw new Exception("Book is currently borrowed by another user.");

        var book = await _context.Books.FindAsync(bookId);
        if (book == null) throw new Exception("Book not found.");

        var borrowRecord = new BorrowRecord{
            UserId = userId,
            BookId = bookId,
            BorrowDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(subscription.BorrowDurationDays),
        };

        _context.BorrowRecords.Add(borrowRecord);
        await _context.SaveChangesAsync();

        return MapToDTO(borrowRecord);
    }

    public async Task<BorrowRecordDTO> ReturnBookAsync(int borrowRecordId)
    {
        var record = await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.BorrowRecordId == borrowRecordId);

        if (record == null)
            throw new Exception("Borrow record not found.");

        if (record.ReturnDate != null)
            throw new Exception("Book has already been returned.");

        record.ReturnDate = DateTime.Now;
        _context.BorrowRecords.Update(record);
        await _context.SaveChangesAsync();

        return MapToDTO(record);
    }

    private BorrowRecordDTO MapToDTO(BorrowRecord record)
    {
        return new BorrowRecordDTO
        {
            BorrowRecordId = record.BorrowRecordId,
            UserId = record.UserId,
            UserName = record.User?.Name,
            BookId = record.BookId,
            BookTitle = record.Book?.Title,
            BorrowDate = record.BorrowDate,
            DueDate = record.DueDate,
            ReturnDate = record.ReturnDate
        };
    }
}