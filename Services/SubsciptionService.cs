using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using Microsoft.EntityFrameworkCore;

public class SubscriptionSerive : ISubscriptionService
{
    public readonly LibraryDbContext _context;

    public SubscriptionSerive(LibraryDbContext context)
    {
        _context = context;
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

    public async Task<Subscription> CreateAsync(Subscription dto){
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
         IsActive = true   
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