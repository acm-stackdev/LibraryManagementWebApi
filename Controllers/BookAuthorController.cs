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
        private readonly ILogger<BookAuthorController> _logger;

        public BookAuthorController(
            IBookAuthorService bookAuthorService,
            ILogger<BookAuthorController> logger)
        {
            _bookAuthorService = bookAuthorService;
            _logger = logger;
        }

        // GET: api/BookAuthor    
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookAuthor>>> GetBookAuthors()
        {
            try
            {
                _logger.LogInformation("Getting all book-author relationships.");

                var list = await _bookAuthorService.GetAllAsync();

                if (!list.Any())
                {
                    _logger.LogWarning("No relationships found.");
                    return NotFound("No relationships found.");
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching relationships: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

         // GET: api/BookAuthor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookAuthor>> GetBookAuthor(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching relationship with ID {id}.");

                var item = await _bookAuthorService.GetByIdAsync(id);

                if (item == null)
                {
                    _logger.LogWarning($"Relationship with ID {id} not found.");
                    return NotFound($"Relationship with ID {id} not found.");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching relationship with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/BookAuthor
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookAuthor>> CreateBookAuthor(BookAuthor bookAuthor)
        {
            try
            {
                if (bookAuthor == null)
                {
                    _logger.LogWarning("BookAuthor object is null.");
                    return BadRequest("Relationship data cannot be null.");
                }

                var created = await _bookAuthorService.CreateAsync(bookAuthor);

                _logger.LogInformation(
                    $"Linked Book {created.BookId} with Author {created.AuthorId} successfully.");

                return CreatedAtAction(
                    nameof(GetBookAuthor),
                    new { id = created.BookAuthorId },
                    created);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating relationship: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/BookAuthor/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookAuthor(int id)
        {
            try
            {
                await _bookAuthorService.DeleteAsync(id);

                _logger.LogInformation($"Relationship with ID {id} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting relationship with ID {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}