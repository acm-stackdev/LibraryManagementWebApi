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
        
        await _context.Entry(wishlist).Reference(w => w.User).LoadAsync();
        await _context.Entry(wishlist).Reference(w => w.Book).LoadAsync();
        await _context.Entry(wishlist.Book!).Collection(b => b.BookAuthors).LoadAsync();

        foreach (var ba in wishlist.Book!.BookAuthors ?? Enumerable.Empty<BookAuthor>())
        {
            await _context.Entry(ba).Reference(ba => ba.Author).LoadAsync();
        }

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

    public async Task<Wishlist?> GetByIdAsync(int wishlistId)
    {
        return await _context.Wishlists
            .Include(w => w.Book)
                .ThenInclude(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
            .Include(w => w.User)
            .FirstOrDefaultAsync(w => w.WishlistId == wishlistId);
    }

    private WishlistDto MapToDto(Wishlist w)
    {
        return new WishlistDto
        {
            WishlistId = w.WishlistId,
            BookId = w.BookId,
            BookTitle = w.Book!.Title ?? "Unknown",
            Authors = w.Book.BookAuthors!.Select(ba => new AuthorDTO
        {
            AuthorId = ba.Author!.AuthorId,
            Name = ba.Author!.Name
        }).ToList(),
            CreatedAt = w.CreatedAt,
            UserId = w.UserId,
            UserEmail = w.User!.Email ?? "Unknown"
        };
    }
}