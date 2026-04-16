using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class WishlistService : IWishlistService
{
    private readonly LibraryContext _context;

    public WishlistService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WishlistDto>> GetWishlistDtosByUserIdAsync(string userId)
    {
        var items = await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Include(w => w.User)
            .Include(w => w.Book)
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
            .ToListAsync();

        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<WishlistDto>> GetAllWishlistDtosAsync()
    {
        var items = await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Book)
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
            .ToListAsync();

        return items.Select(MapToDto);
    }

    public async Task<Wishlist> AddToWishlistAsync(string userId, int bookId)
    {
        var bookExists = await _context.Books.AnyAsync(b => b.BookId == bookId);
        if (!bookExists)
        {
            throw new KeyNotFoundException($"Book with ID {bookId} not found.");
        }

        var existing = await _context.Wishlists
            .Include(w => w.Book)
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

        if (existing != null) return existing;

        var wishlist = new Wishlist
        {
            UserId = userId,
            BookId = bookId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();
        
        // Re-fetch to load all navigation properties including nested ones
        return await GetByIdAsync(wishlist.WishlistId) ?? wishlist;
    }

    public async Task RemoveFromWishlistAsync(int wishlistId)
    {
        var wishlist = await _context.Wishlists.FindAsync(wishlistId);
        if (wishlist != null)
        {
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Wishlist?> GetByIdAsync(int wishlistId)
    {
        return await _context.Wishlists
            .Include(w => w.Book)
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.WishlistId == wishlistId);
    }

    public async Task<IEnumerable<WishlistSummaryDto>> GetWishlistSummaryAsync()
    {
        return await _context.Wishlists
            .GroupBy(w => new { w.BookId, w.Book!.Title })
            .Select(g => new WishlistSummaryDto
            {
                BookId = g.Key.BookId,
                BookTitle = g.Key.Title,
                WishlistCount = g.Count()
            })
            .ToListAsync();
    }

    private WishlistDto MapToDto(Wishlist w)
    {
        return new WishlistDto
        {
            WishlistId = w.WishlistId,
            BookId = w.BookId,
            BookTitle = w.Book?.Title ?? "Unknown",
            Authors = w.Book?.BookAuthors?.Select(ba => new AuthorDTO
            {
                AuthorId = ba.Author?.AuthorId ?? 0,
                Name = ba.Author?.Name ?? "Unknown"
            }).ToList() ?? new List<AuthorDTO>(),
            CreatedAt = w.CreatedAt,
            UserId = w.UserId,
            UserEmail = w.User?.Email ?? "Unknown"
        };
    }
}