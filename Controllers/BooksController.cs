using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
                var books = await _bookService.GetAllAsync();
                if(!books.Any()){
                    return NotFound("No books found.");
                }

                var bookDtos = books.Select(b => new BookDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    CategoryId = b.CategoryId,
                    PublishedYear = b.PublishedYear,
                    Description = b.Description,
                    TotalPages = b.TotalPages,
                    CategoryName = b.Category?.Name,
                    AuthorNames = b.BookAuthors.Select(ba => ba.Author?.Name ?? "Unknown").ToList()
                });

                return Ok(bookDtos);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if(book == null){
                return NotFound($"Book with ID {id} not found.");
            }

            var bookDto = new BookDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                ISBN = book.ISBN,
                CategoryId = book.CategoryId,
                PublishedYear = book.PublishedYear,
                Description = book.Description,
                TotalPages = book.TotalPages,
                CategoryName = book.Category?.Name,
                AuthorNames = book.BookAuthors.Select(ba => ba.Author?.Name ?? "Unknown").ToList()
            };

            return Ok(bookDto);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDTO dto)
        {
                if(dto.AuthorNames == null || !dto.AuthorNames.Any())
                {
                    return BadRequest("Book must have at least one author.");
                }

                var book = await _bookService.GetByIdAsync(id);
                if(book == null)
                {
                    return NotFound($"Book with ID {id} not found.");
                }

                book.Title = dto.Title;
                book.ISBN = dto.ISBN;
                book.CategoryId = dto.CategoryId;
                book.PublishedYear = dto.PublishedYear;
                book.Description = dto.Description;
                book.TotalPages = dto.TotalPages;

                await _bookService.UpdateBookAsync(book, dto.AuthorNames);
                return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDTO>> CreateBook(CreateBookDTO dto)
        {
                if(dto == null){
                    return BadRequest("Book cannot be null.");
                }

                if(dto.AuthorNames == null || !dto.AuthorNames.Any()){
                    return BadRequest("Author names are required.");
                }

                var existingBook = await _bookService.GetByISBNAsync(dto.ISBN);
                if(existingBook != null)
                {
                    return Conflict($"Book with ISBN {dto.ISBN} already exists.");
                }

                var book = new Book{
                    Title = dto.Title,
                    ISBN = dto.ISBN,
                    CategoryId = dto.CategoryId,
                    PublishedYear = dto.PublishedYear,
                    Description = dto.Description,
                    TotalPages = dto.TotalPages,
                };
                
                var createdBook = await _bookService.CreateBookAsync(book, dto.AuthorNames);

                var bookDto = new BookDTO
                {
                    BookId = createdBook.BookId,
                    Title = createdBook.Title,
                    ISBN = createdBook.ISBN,
                    CategoryId = createdBook.CategoryId,
                    PublishedYear = createdBook.PublishedYear,
                    Description = createdBook.Description,
                    TotalPages = createdBook.TotalPages,
                    CategoryName = createdBook.Category?.Name,
                    AuthorNames = createdBook.BookAuthors.Select(ba => ba.Author?.Name ?? "Unknown").ToList()
                };

                return CreatedAtAction(nameof(GetBook), new { id = bookDto.BookId }, bookDto);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
                await _bookService.DeleteBookAsync(id);
                return NoContent();
        }
    }
}
