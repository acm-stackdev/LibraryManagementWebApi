using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SubscriptionService : ISubscriptionService
{
    private readonly LibraryContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<SubscriptionService> _logger;
    private const int DEFAULT_MAX_BORROW = 3;
    private const int DEFAULT_DURATION_DAYS = 7;

    public SubscriptionService(LibraryContext context, UserManager<AppUser> userManager, ILogger<SubscriptionService> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions.Include(s => s.User).ToListAsync();
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

    public async Task<Subscription> CreateOrUpdateAsync(SubscriptionDTO dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if(user == null){
            throw new Exception("User not found");
        }

        var now = DateTime.Now;
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == dto.UserId);

        if(subscription != null)
        {
            if(subscription.EndDate > now)
            {
                subscription.EndDate = subscription.EndDate.AddMonths(dto.NumberofMonths);
            }
            else
            {
                subscription.StartDate = now;
                subscription.EndDate = now.AddMonths(dto.NumberofMonths);
                subscription.IsActive = true;
            }
        }else
        {
            subscription = new Subscription
            {
                UserId = dto.UserId,
                MaxBorrowLimit = DEFAULT_MAX_BORROW,
                BorrowDurationDays = DEFAULT_DURATION_DAYS,
                StartDate = now,
                EndDate = now.AddMonths(dto.NumberofMonths),
                IsActive = true
            };
            _context.Subscriptions.Add(subscription);
        }  
        await _context.SaveChangesAsync();

        var currentRoles = await _userManager.GetRolesAsync(user);
        if(!currentRoles.Contains("Member"))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, "Member");
        } 
        return subscription;
    }

    public async Task DeleteAsync(string userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId);

            if(subscription == null)
            {
                throw new Exception("Subscription not found");
            }

            var user = await _userManager.FindByIdAsync(subscription.UserId);
    if (user != null)
    {
        var roles = await _userManager.GetRolesAsync(user);

        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user, "User");
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

        var user = await _userManager.FindByIdAsync(subscription.UserId);
        if(user == null){
            return false;
        }

        var now = DateTime.Now;

        if(subscription.EndDate < now)
        {
            subscription.IsActive = false;
            await _context.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, "User");

            return false;
        }

        var rolesNow = await _userManager.GetRolesAsync(user);
        if (!rolesNow.Contains("Member"))
        {
            await _userManager.RemoveFromRolesAsync(user, rolesNow);
            await _userManager.AddToRoleAsync(user, "Member");
        }
        return true;
    }
}