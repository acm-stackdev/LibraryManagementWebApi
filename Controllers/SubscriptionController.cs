using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // GET: api/Subscription
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
                var subscription = await _subscriptionService.GetAllAsync();
                if(subscription == null) return NotFound();
                return Ok(subscription);
        }

        // GET: api/Subscription/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Subscription>> GetSubscription(int id)
        {
                var subscription = await _subscriptionService.GetByIdAsync(id);
                if(subscription == null) return NotFound();
                return Ok(subscription);
        }

        // PUT: api/Subscription
        // Update subscription or create new subscription
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateOrUpdateSubscription([FromBody] SubscriptionDTO dto)
        {
                var subscription = await _subscriptionService.CreateOrUpdateAsync(dto);
                return Ok(subscription);
        }

        //DELETE : api/Subscription/5
        [HttpDelete("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubscription(string userId)
        {
                var subscription = await _subscriptionService.GetByUserIdAsync(userId);
                if(subscription == null)
                {
                    return NotFound("Subscription not found");
                }

                await _subscriptionService.DeleteAsync(userId);

                return Ok($"Subscription for user {subscription.UserId} deleted and role downgraded to User.");
        }

        // GET: api/Subscription/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMySubscription()
        {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Automatically update subscription status
                await _subscriptionService.IsSubscriptionValidAsync(userId);

                var subscription = await _subscriptionService.GetByUserIdAsync(userId);
                if (subscription == null)
                    return NotFound("You have no subscription");

                var remainingDays = (subscription.EndDate - DateTime.Now).Days;
                return Ok(new
                {
                    subscription.UserId,
                    subscription.StartDate,
                    subscription.EndDate,
                    subscription.MaxBorrowLimit,
                    subscription.BorrowDurationDays,
                    subscription.IsActive,
                    RemainingDays = remainingDays > 0 ? remainingDays : 0
                });
        }
    }
}
