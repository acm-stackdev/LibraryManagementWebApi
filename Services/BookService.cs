using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApi.Services
{
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

        public async Task<BookDTO?> GetByISBNAsync(string isbn)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.ISBN == isbn);

            return book != null ? MapToDTO(book) : null;
        }

        // Create a new book and handle authors
        public async Task<BookDTO> CreateBookAsync(CreateBookDTO dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                ISBN = dto.ISBN,
                CategoryId = dto.CategoryId,
                PublishedYear = dto.PublishedYear,
                Description = dto.Description,
                TotalPages = dto.TotalPages,
                BookAuthors = new List<BookAuthor>()
            };

            await HandleAuthorsAsync(book, dto.AuthorNames);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Return fully loaded DTO
            return await GetByIdAsync(book.BookId);
        }

        // Update book details and authors
        public async Task<BookDTO?> UpdateBookAsync(int id, UpdateBookDTO dto)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return null;

            book.Title = dto.Title;
            book.ISBN = dto.ISBN;
            book.CategoryId = dto.CategoryId;
            book.PublishedYear = dto.PublishedYear;
            book.Description = dto.Description;
            book.TotalPages = dto.TotalPages;

            // Remove existing author links
            _context.BookAuthors.RemoveRange(book.BookAuthors);
            book.BookAuthors.Clear();

            // Handle new author links
            await HandleAuthorsAsync(book, dto.AuthorNames);

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return await GetByIdAsync(book.BookId);
        }

        private async Task HandleAuthorsAsync(Book book, List<string> authorNames)
        {
            foreach (var name in authorNames)
            {
                var normalizedName = name.Trim();
                var existingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.Name.ToLower() == normalizedName.ToLower());

                if (existingAuthor == null)
                {
                    existingAuthor = new Author { Name = normalizedName };
                    _context.Authors.Add(existingAuthor);
                    // EF will handle the author creation when saving the book/book-author link
                }

                book.BookAuthors.Add(new BookAuthor { Author = existingAuthor });
            }
        }

        // Delete a book
        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                // Related BookAuthors are usually deleted via Cascade Delete if configured, 
                // but let's be explicit if needed.
                var links = _context.BookAuthors.Where(ba => ba.BookId == id);
                _context.BookAuthors.RemoveRange(links);

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}