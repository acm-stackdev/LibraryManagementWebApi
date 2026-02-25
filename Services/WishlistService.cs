using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class WishlistService : IWishlistService
{
    private readonly LibraryContext _context;

    public WishlistService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Wishlist>> GetAllAsync()
    {
        return await _context.Wishlists
            .Include(w => w.Book)
            .Include(w => w.User)
            .ToListAsync();
    }

    public async Task<Wishlist?> GetByIdAsync(int id)
    {
        return await _context.Wishlists
            .Include(w => w.Book)
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.BookId == id);
    }

    public async Task<IEnumerable<Wishlist>> GetWishlistByUserIdAsync(string userId)
    {
        return await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Include(w => w.Book)
            .ToListAsync();
    }

    public async Task<Wishlist> AddToWishlistAsync(string userId, int bookId)
    {
        // Check if the book is already in the wishlist
        var existing = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

        if (existing != null) return existing;

        var wishlist = new Wishlist
        {
            UserId = userId,
            BookId = bookId,
            CreatedAt = DateTime.Now
        };

        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();

        return wishlist;
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
}