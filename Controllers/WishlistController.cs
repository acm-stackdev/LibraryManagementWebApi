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

        // GET: api/Wishlist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WishlistDto>>> GetMyWishlists()
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                if (User.IsInRole("Admin"))
                {
                    var allItems = await _wishlistService.GetAllWishlistDtosAsync();
                    return Ok(allItems);
                }

                var items = await _wishlistService.GetWishlistDtosByUserIdAsync(userId);
                return Ok(items);
        }

        // POST: api/Wishlist/{bookId}
        [HttpPost("{bookId}")]
        public async Task<ActionResult<Wishlist>> AddToWishlist(int bookId)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var item = await _wishlistService.AddToWishlistAsync(userId, bookId);

                // Map entity to DTO using the service helper
                var dto = new WishlistDto
                {
                    WishlistId = item.WishlistId,
                    BookId = item.BookId,
                    BookTitle = item.Book!.Title,
                    Authors = item.Book.BookAuthors!.Select(ba => new AuthorDTO
                    {
                        AuthorId = ba.Author!.AuthorId,
                        Name = ba.Author!.Name
                    }).ToList(),
                    CreatedAt = item.CreatedAt
                };

                return CreatedAtAction(nameof(GetMyWishlists), dto);
        }

        // DELETE: api/Wishlist/{wishlistId}
        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var wishlist = await _wishlistService.GetByIdAsync(wishlistId);
                if (wishlist == null) return NotFound();

                if (wishlist.UserId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                await _wishlistService.RemoveFromWishlistAsync(wishlistId);
                return NoContent();
        }
    }
}
