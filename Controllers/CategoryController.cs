using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try{
                var categories = await _categoryService.GetAllAsync();
                if(!categories.Any()){
                    return NotFound("No categories found.");
                }
                return Ok(categories);
            }catch(Exception ex){
                _logger.LogError($"Error getting categories: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
           try{
            var category = await _categoryService.GetByIdAsync(id);
            if(category == null){
                return NotFound($"Category not found with id: {id}");
            }
            return Ok(category);
           }catch(Exception ex){
            _logger.LogError($"Error getting category: {ex.Message}");
            return StatusCode(500, "Internal server error");
           }
        }

        // PUT: api/Category/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
           try{
            if(id != category.CategoryId){
                return BadRequest("Category ID mismatch");
            }
            await _categoryService.UpdateAsync(category);
            return NoContent();
           }catch(Exception ex){
            _logger.LogError($"Error updating category: {ex.Message}");
            return StatusCode(500, "Internal server error");
           }
        }

        // POST: api/Category
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
           try{
            var newCategory = await _categoryService.CreateAsync(category);
            return CreatedAtAction(nameof(GetCategory), new{id = newCategory.CategoryId}, newCategory);
           }catch(Exception ex){
            _logger.LogError($"Error creating category: {ex.Message}");
            return StatusCode(500, "Internal server error");
           }
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try{
                await _categoryService.DeleteAsync(id);
                return NoContent();
            }catch(Exception ex){
                _logger.LogError($"Error deleting category: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
