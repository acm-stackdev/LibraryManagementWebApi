using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;

public interface ISubscriptionService
{
     Task<IEnumerable<Subscription>> GetAllAsync();
    Task<Subscription?> GetByIdAsync(int id);
    Task<Subscription?> GetByUserIdAsync(string userId);

    Task<Subscription> CreateAsync(SubscriptionDTO dto);
    Task UpdateAsync(int id, SubscriptionDTO dto);
    Task DeleteAsync(int id);

    Task<bool> IsSubscriptionValidAsync(string userId);
}