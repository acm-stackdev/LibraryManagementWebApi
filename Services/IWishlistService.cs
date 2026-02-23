using LibraryManagementSystem.Models;

public interface IWishlistService
{
    Task<IEnumerable<Wishlist>> GetAllAsync();
    Task<Wishlist?> GetByIdAsync(int id);
    Task<Wishlist> AddToWishlistAsync(string userId, int bookId);
    Task RemoveFromWishlistAsync(int wishlistId);
}