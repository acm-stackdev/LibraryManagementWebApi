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

    // Create a new book and handle authors 
    public async Task<Book> CreateBookAsync(Book book, List<string> authorNames)
    {
        var categoryName = book.Category?.Name;
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            if (book.CategoryId != 0)
            {
                categoryName = await _context.Categories
                    .Where(c => c.CategoryId == book.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
            }
        }

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Category name is required to create a book.");
        }

        return await CreateBookAsync(book, categoryName, authorNames);
    }

    // Create a new book and handle authors and category
    public async Task<Book> CreateBookAsync(Book book, string categoryName, List<string> authorNames)
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

        book.BookAuthors = new List<BookAuthor>();

        foreach (var name in authorNames)
        {
             var normalizedName = name.Trim().ToUpper();
            var existingAuthor = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.ToUpper() == normalizedName);

            if (existingAuthor == null)
            {
                var newAuthor = new Author { Name = normalizedName };
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

    public async Task<Book?> GetByISBNAsync(string isbn)
    {
        return await _context.Books
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .FirstOrDefaultAsync(b => b.ISBN == isbn);  
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
            var normalizedName = name.Trim().ToUpper();
            var existingAuthor = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name.ToUpper() == normalizedName);

            if (existingAuthor == null)
            {
                var newAuthor = new Author { Name = normalizedName };
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

    public async Task UpdateBookAsync(Book book, List<string> authorNames)
    {
        var categoryName = book.Category?.Name;
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            if (book.CategoryId != 0)
            {
                categoryName = await _context.Categories
                    .Where(c => c.CategoryId == book.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
            }
        }

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Category name is required to update a book.");
        }

        await UpdateBookAsync(book, categoryName, authorNames);
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