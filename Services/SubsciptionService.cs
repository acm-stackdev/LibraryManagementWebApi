using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class SubscriptionService : ISubscriptionService
{
    public readonly LibraryContext _context;
    public readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(LibraryContext context, ILogger<SubscriptionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<Subscription?> GetByIdAsync(int id)
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.SubscriptionId == id);
    }

    public async Task<Subscription?> GetByUserIdAsync(string userId)
    {
        return await _context.Subscriptions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<Subscription> CreateAsync(SubscriptionDTO dto)
    {
        var existing = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == dto.UserId && s.IsActive);
        if(existing != null){
                throw new Exception("User already has an active subscription");
        }

        var subscription = new Subscription{
         UserId = dto.UserId,
         MaxBorrowLimit = dto.MaxBorrowLimit,
         BorrowDurationDays = dto.BorrowDurationDays,
         StartDate = DateTime.Now,
         EndDate = DateTime.Now.AddMonths(1),
         IsActive = dto.IsActive   
        };
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task UpdateAsync(int id, SubscriptionDTO dto)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);

        if(subscription == null){
            throw new Exception("Subscription not found");
        }

        subscription.MaxBorrowLimit = dto.MaxBorrowLimit;
        subscription.BorrowDurationDays = dto.BorrowDurationDays;
        subscription.StartDate = dto.StartDate;
        subscription.EndDate = dto.EndDate;
        subscription.IsActive = dto.IsActive;

        _context.Entry(subscription).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id){
        var subscription = await _context.Subscriptions.FindAsync(id);

        if(subscription == null){
            throw new Exception("Subscription not found");
        }
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsSubscriptionValidAsync(string userId){
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if(subscription == null){
            return false;
        }

        if(subscription.EndDate < DateTime.Now){
            subscription.IsActive = false;
            await _context.SaveChangesAsync();
            return false;
        }
        return subscription.IsActive;
    }
}