using System.Security.Claims;
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
    [Authorize]
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

        // GET: api/Wishlist for current user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WishlistDto>>> GetMyWishlists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var items = await _wishlistService.GetWishlistDtosByUserIdAsync(userId);
            return Ok(items);
        }

        // GET: api/Wishlist/admin/all
        // Admin only: View every user's wishlist items
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<WishlistDto>>> GetAllWishlists()
        {
            var allItems = await _wishlistService.GetAllWishlistDtosAsync();
            return Ok(allItems);
        }

        // GET: api/Wishlist/admin/summary
        // Admin only: View which books are most popular in wishlists
        [HttpGet("admin/summary")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<WishlistSummaryDto>>> GetWishlistSummary()
        {
            var summary = await _wishlistService.GetWishlistSummaryAsync();
            return Ok(summary);
        }

        // POST: api/Wishlist/{bookId}
        [HttpPost("{bookId}")]
        public async Task<ActionResult<WishlistDto>> AddToWishlist(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var item = await _wishlistService.AddToWishlistAsync(userId, bookId);

            var dto = new WishlistDto
            {
                WishlistId = item.WishlistId,
                BookId = item.BookId,
                BookTitle = item.Book?.Title ?? "Unknown",
                Authors = item.Book?.BookAuthors?.Select(ba => new AuthorDTO
                {
                    AuthorId = ba.Author?.AuthorId ?? 0,
                    Name = ba.Author?.Name ?? "Unknown"
                }).ToList() ?? new List<AuthorDTO>(),
                CreatedAt = item.CreatedAt,
                UserId = item.UserId,
                UserEmail = item.User?.Email ?? "Unknown"
            };

            return CreatedAtAction(nameof(GetMyWishlists), dto);
        }

        // DELETE: api/Wishlist/{wishlistId} for current user even for admin
        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var wishlist = await _wishlistService.GetByIdAsync(wishlistId);
            if (wishlist == null) return NotFound();

            // Ownership check (Admins cannot delete other users' wishlist items)
            if (wishlist.UserId != userId)
                return Forbid();

            await _wishlistService.RemoveFromWishlistAsync(wishlistId);
            return NoContent();
        }
    }
}
