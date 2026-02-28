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
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            try{
                _logger.LogInformation("Getting all books with related data.");
                var books = await _bookService.GetAllAsync();
                if(!books.Any()){
                    _logger.LogWarning("No books found.");
                    return NotFound("No books found.");
                }
                return Ok(books);
            }
            catch(Exception ex){
                _logger.LogError($"Error getting books: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
           try{
            _logger.LogInformation($"Fetching book with ID{id}.");
            var book = await _bookService.GetByIdAsync(id);
            if(book == null){
                _logger.LogWarning($"Book with ID{id} not found.");
                return NotFound($"Book with ID{id} not found.");
            }
            return Ok(book);
           }
           catch(Exception ex){
            _logger.LogError($"Error fetching book with ID{id}: {ex.Message}");
            return StatusCode(500, "Internal server error");
           }
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            try
            {
                if (id != book.BookId)
                {
                    _logger.LogWarning("Book ID mismatch.");
                    return BadRequest("Book ID mismatch.");
                }

                await _bookService.UpdateBookAsync(book, new List<string>());
                _logger.LogInformation($"Book with ID {id} updated.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Books
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Book>> CreateBook(Book book)
        {
            try{
                if(book == null){
                    _logger.LogWarning("Book object is null.");
                    return BadRequest("Book cannot be null.");
                }

                var createdBook = await _bookService.CreateBookAsync(book, new List<string>());
                _logger.LogInformation($"Book with ID{createdBook.BookId} created successfully.");
                return CreatedAtAction(nameof(GetBook), new {id = createdBook.BookId}, createdBook);
                
            }
            catch(Exception ex){
                _logger.LogError($"Error creating book: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
           try
            {
                await _bookService.DeleteBookAsync(id);
                _logger.LogInformation($"Book with ID {id} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
