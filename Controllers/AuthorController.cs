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
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }

        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            try{
                var authors = await _authorService.GetAllAsync();
                if(!authors.Any()){
                    return NotFound("No authors found");
                }
                return Ok(authors);
            }catch(Exception ex){
                _logger.LogError($"Error fetching authors: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Author/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            try{
                var author = await _authorService.GetByIdAsync(id);
                if(author == null){
                    return NotFound($"Author with id {id} not found");
                }
                return Ok(author);
            }catch(Exception ex){
                _logger.LogError($"Error fetching author: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Author/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, Author author)
        {
            try{
                if(id != author.AuthorId){
                    return BadRequest("Author ID mismatch.");
                }
                await _authorService.UpdateAsync(author);
                return Ok(author);
            }catch(Exception ex){
                _logger.LogError($"Error updating author: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Author
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Author>> CreateAuthor(Author author)
        {
            try{
                 var created = await _authorService.CreateAsync(author);
                return CreatedAtAction(nameof(GetAuthor), new { id = created.AuthorId }, created);
            }catch(Exception ex){
                _logger.LogError($"Error creating author: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Author/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try{
                await _authorService.DeleteAsync(id);
                return Ok();
            }catch(Exception ex){
                _logger.LogError($"Error deleting author: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
