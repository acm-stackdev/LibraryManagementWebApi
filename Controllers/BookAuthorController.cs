using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookAuthorController : ControllerBase
    {
        private readonly IBookAuthorService _bookAuthorService;

        public BookAuthorController(
            IBookAuthorService bookAuthorService)
        {
            _bookAuthorService = bookAuthorService;
        }

        // GET: api/BookAuthor    
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookAuthor>>> GetBookAuthors()
        {
                var list = await _bookAuthorService.GetAllAsync();

                if (!list.Any())
                {
                    return NotFound("No relationships found.");
                }

                return Ok(list);
        }

         // GET: api/BookAuthor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookAuthor>> GetBookAuthor(int id)
        {
                var item = await _bookAuthorService.GetByIdAsync(id);

                if (item == null)
                {
                    return NotFound($"Relationship with ID {id} not found.");
                }

                return Ok(item);
        }

        // POST: api/BookAuthor
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookAuthor>> CreateBookAuthor(BookAuthor bookAuthor)
        {
                if (bookAuthor == null)
                {
                    return BadRequest("Relationship data cannot be null.");
                }

                var created = await _bookAuthorService.CreateAsync(bookAuthor);

                return CreatedAtAction(
                    nameof(GetBookAuthor),
                    new { id = created.BookAuthorId },
                    created);
        }

        // DELETE: api/BookAuthor/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookAuthor(int id)
        {
                await _bookAuthorService.DeleteAsync(id);

                return NoContent();
        }
    }
}