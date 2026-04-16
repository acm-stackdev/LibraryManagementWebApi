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
using BackendApi.Services;

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
            if (books == null || !books.Any())
            {
                return NotFound("No books found.");
            }
            return Ok(books);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }
            return Ok(book);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDTO dto)
        {
            if (dto.AuthorNames == null || !dto.AuthorNames.Any())
            {
                return BadRequest("Book must have at least one author.");
            }

            var updatedBook = await _bookService.UpdateBookAsync(id, dto);
            if (updatedBook == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDTO>> CreateBook(CreateBookDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Book data is required.");
            }

            if (dto.AuthorNames == null || !dto.AuthorNames.Any())
            {
                return BadRequest("At least one author name is required.");
            }

            var existingBook = await _bookService.GetByISBNAsync(dto.ISBN);
            if (existingBook != null)
            {
                return Conflict($"Book with ISBN {dto.ISBN} already exists.");
            }

            var createdBookDto = await _bookService.CreateBookAsync(dto);
            return CreatedAtAction(nameof(GetBook), new { id = createdBookDto.BookId }, createdBookDto);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
    }
}