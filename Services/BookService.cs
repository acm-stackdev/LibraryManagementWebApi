using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using Microsoft.EntityFrameworkCore;

public class BookService : IBookService
{
    private readonly LibraryContext _context;

    public BookService(LibraryContext context)
    {
        _context = context;
    }

    // Map Book entity to BookDTO
    private BookDTO MapToDTO(Book book)
    {
        return new BookDTO
        {
            BookId = book.BookId,
            Title = book.Title,
            ISBN = book.ISBN,
            CategoryId = book.CategoryId,
            PublishedYear = book.PublishedYear,
            Description = book.Description,
            TotalPages = book.TotalPages,
            CategoryName = book.Category?.Name,
            AuthorNames = book.BookAuthors?.Select(ba => ba.Author?.Name ?? "Unknown").ToList() ?? new List<string>()
        };
    }

    // Get all books as DTOs
    public async Task<IEnumerable<BookDTO>> GetAllAsync()
    {
        var books = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .ToListAsync();

        return books.Select(MapToDTO);
    }

    // Get one book by Id as DTO
    public async Task<BookDTO?> GetByIdAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
            .FirstOrDefaultAsync(b => b.BookId == id);

        return book != null ? MapToDTO(book) : null;
    }

    // Create a new book and handle authors 
    public async Task<BookDTO> CreateBookAsync(Book book, List<string> authorNames)
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

        var createdBook = await CreateBookAsync(book, categoryName, authorNames);
        return MapToDTO(createdBook);
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

    // Update book details and authors
    public async Task UpdateBookAsync(int id, UpdateBookDTO dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) throw new Exception($"Book with ID {id} not found.");

        book.Title = dto.Title;
        book.ISBN = dto.ISBN;
        book.CategoryId = dto.CategoryId;
        book.PublishedYear = dto.PublishedYear;
        book.Description = dto.Description;
        book.TotalPages = dto.TotalPages;

        var existingLinks = _context.BookAuthors.Where(ba => ba.BookId == book.BookId);
        _context.BookAuthors.RemoveRange(existingLinks);

        book.BookAuthors = new List<BookAuthor>();
        foreach (var name in dto.AuthorNames)
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