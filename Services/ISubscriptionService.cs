using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;

public interface ISubscriptionService
{
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<Subscription?> GetByIdAsync(int id);
    Task<Subscription?> GetByUserIdAsync(string userId);
    Task<Subscription> CreateOrUpdateAsync(SubscriptionDTO dto);
    Task DeleteAsync(string userId);
    Task<bool> IsSubscriptionValidAsync(string userId);

}