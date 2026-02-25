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
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        // GET: api/Wishlist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetWishlists()
        {
            try
            {
                var items = await _wishlistService.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlists items.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Wishlist/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Wishlist>> GetUserWishlist(string userId)
        {
           try
           {
                var items = await _wishlistService.GetWishlistByUserIdAsync(userId); 
                return Ok(items);           
           }catch (Exception ex)
           {
                _logger.LogError(ex, $"Error fetching wishlist for user {userId}.");
                return StatusCode(500, "Internal server error");
           }
        }

        // POST: api/Wishlist/{userId}/{bookId}
        [HttpPost("{userId}/{bookId}")]
        public async Task<ActionResult<Wishlist>> AddToWishlist(string userId, int bookId)
        {
            try
            {
                var item = await _wishlistService
                 .AddToWishlistAsync(userId, bookId);
                 return CreatedAtAction(nameof(GetUserWishlist), new { userId }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding book {bookId} to wishlist for user {userId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Wishlist/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
           try
           {
            await _wishlistService.RemoveFromWishlistAsync(id);
            return NoContent();
           }
           catch (Exception ex)
           {
                _logger.LogError(ex, $"Error removing book {id} from wishlist.");
                return StatusCode(500, "Internal server error");
           }
        }
    }
}
