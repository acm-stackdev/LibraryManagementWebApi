using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class BookService : IBookService
{
    private readonly LibraryContext _context;

    public BookService(LibraryContext context)
    {
        _context = context;
    }

    // Get all books with related data
    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .ToListAsync();
    }

    // Get one book by Id with related data
    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .FirstOrDefaultAsync(b => b.BookId == id);
    }

    // Create a new book and handle authors and category
    public async Task<Book> CreateBookAsync(Book book,string categoryName, List<string> authorNames)
    {

        book.BookAuthors = new List<BookAuthor>();

        foreach (var name in authorNames)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == categoryName);
                
            if(category == null)    
            {
                var newCategory = new Category { Name = categoryName };
                _context.Categories.Add(newCategory);
                await _context.SaveChangesAsync();
            }

            book.CategoryId = category.CategoryId;

            var existingAuthor = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

            if (existingAuthor == null)
            {

                var newAuthor = new Author { Name = name };
                _context.Authors.Add(newAuthor);
                await _context.SaveChangesAsync();
                book.BookAuthors.Add(new BookAuthor { AuthorId = newAuthor.AuthorId });
            }
            else
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = existingAuthor.AuthorId });
            }
        }

        // Add the book
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    // Update book details and authors with category name
    public async Task UpdateBookAsync(Book book, string categoryName, List<string> authorNames)
    {

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);

        if (category == null)
        {
            category = new Category { Name = categoryName };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        book.CategoryId = category.CategoryId;

        var existingLinks = _context.BookAuthors.Where(ba => ba.BookId == book.BookId);
        _context.BookAuthors.RemoveRange(existingLinks);


        book.BookAuthors = new List<BookAuthor>();
        foreach (var name in authorNames)
        {
            var existingAuthor = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

            if (existingAuthor == null)
            {
                var newAuthor = new Author { Name = name };
                _context.Authors.Add(newAuthor);
                await _context.SaveChangesAsync();
                book.BookAuthors.Add(new BookAuthor { AuthorId = newAuthor.AuthorId });
            }
            else
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = existingAuthor.AuthorId });
            }
        }

        // Update book entity
        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    // Delete a book
    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            // Remove related BookAuthors first
            var links = _context.BookAuthors.Where(ba => ba.BookId == id);
            _context.BookAuthors.RemoveRange(links);

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}