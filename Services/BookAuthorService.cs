using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class BookAuthorService : IBookAuthorService
{
    private readonly LibraryContext _context;

    public BookAuthorService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookAuthor>> GetAllAsync()
    {
        return await _context.BookAuthors
            .Include(ba => ba.Book)
            .Include(ba => ba.Author)
            .ToListAsync();
    }

    public async Task<BookAuthor?> GetByIdAsync(int id)
    {
        return await _context.BookAuthors
            .Include(ba => ba.Book)
            .Include(ba => ba.Author)
            .FirstOrDefaultAsync(ba => ba.BookAuthorId == id);
    }    

    public async Task<BookAuthor> CreateAsync(BookAuthor bookAuthor)
    {

        var bookExists = await _context.Books
            .AnyAsync(b => b.BookId == bookAuthor.BookId);

        if (!bookExists)
            throw new ArgumentException("Book does not exist.");


        var authorExists = await _context.Authors
            .AnyAsync(a => a.AuthorId == bookAuthor.AuthorId);

        if (!authorExists)
            throw new ArgumentException("Author does not exist.");


        var alreadyLinked = await _context.BookAuthors
            .AnyAsync(ba =>
                ba.BookId == bookAuthor.BookId &&
                ba.AuthorId == bookAuthor.AuthorId);

        if (alreadyLinked)
            throw new ArgumentException("This author is already linked to this book.");

        _context.BookAuthors.Add(bookAuthor);
        await _context.SaveChangesAsync();

        return bookAuthor;
    }

     public async Task DeleteAsync(int id)
    {
        var link = await _context.BookAuthors.FindAsync(id);
        if (link != null)
        {
            _context.BookAuthors.Remove(link);
            await _context.SaveChangesAsync();
        }
    }
}