using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
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
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
                var books = await _bookService.GetAllAsync();
                if(!books.Any()){
                    return NotFound("No books found.");
                }
                return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if(book == null){
                return NotFound($"Book with ID{id} not found.");
            }
            return Ok(book);
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

                await _bookService.UpdateBookAsync(book, dto.AuthorNames);
                return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> CreateBook(CreateBookDTO dto)
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
                };
                
                var createdBook = await _bookService.CreateBookAsync(book, dto.AuthorNames);
                return CreatedAtAction(nameof(GetBook), new { id = createdBook.BookId }, createdBook);
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
