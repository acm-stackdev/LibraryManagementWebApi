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

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {

                var authors = await _authorService.GetAllAsync();
                if(!authors.Any()){
                    return NotFound("No authors found");
                }
                return Ok(authors);
        }

        // GET: api/Author/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
                var author = await _authorService.GetByIdAsync(id);
                if(author == null){
                    return NotFound($"Author with id {id} not found");
                }
                return Ok(author);
        }

        // PUT: api/Author/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, Author author)
        {
                if(id != author.AuthorId){
                    return BadRequest("Author ID mismatch.");
                }
                await _authorService.UpdateAsync(author);
                return Ok(author);
        }

        // POST: api/Author
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Author>> CreateAuthor(Author author)
        {
                 var created = await _authorService.CreateAsync(author);
                return CreatedAtAction(nameof(GetAuthor), new { id = created.AuthorId }, created);
        }

        // DELETE: api/Author/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
                await _authorService.DeleteAsync(id);
                return Ok();
        }
    }
}
