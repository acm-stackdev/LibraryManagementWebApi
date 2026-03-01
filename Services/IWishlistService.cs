using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;

public interface IWishlistService
{
    Task<IEnumerable<WishlistDto>> GetWishlistDtosByUserIdAsync(string userId);
    Task<IEnumerable<WishlistDto>> GetAllWishlistDtosAsync();
    Task<Wishlist> AddToWishlistAsync(string userId, int bookId);
    Task RemoveFromWishlistAsync(int wishlistId);
    Task<Wishlist?> GetByIdAsync(int wishlistId);
}