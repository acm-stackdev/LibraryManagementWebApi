using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        public readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(ISubscriptionService subscriptionService, ILogger<SubscriptionController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        // GET: api/Subscription
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            try
            {
                var subscription = await _subscriptionService.GetAllAsync();
                return Ok(subscription);
            }catch(Exception ex)
            {
                _logger.LogError($"Error fetching subscriptions: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Subscription/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscription(int id)
        {
            try
            {
                var subscription = await _subscriptionService.GetByIdAsync(id);
                if(subscription == null) return NotFound();
                return Ok(subscription);
            }catch(Exception ex)
            {
                _logger.LogError($"Error fetching subscription: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Subscription/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubscription(int id, Subscription subscription)
        {
            if (id != subscription.SubscriptionId)
            {
                return BadRequest();
            }

            try
            {
                await _subscriptionService.UpdateAsync(subscription);
                return _logger.LogInformation($"Subscription updated successfully: {subscription.SubscriptionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating subscription: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Subscription
        [HttpPost]
        public async Task<ActionResult<Subscription>> CreateSubscription([FromBody] Subscription subscription)
        {
            try
            {
                var newSubscription = await _subscriptionService.createAsync(subscription);
                return CreatedAtAction(nameof(GetSubscription), new {id = newSubscription.SubscriptionId}, newSubscription);
            }catch(Exception ex)
            {
                _logger.LogError($"Error creating subscription for user {subscription.UserId}.");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Subscription/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            try
            {
                await _subscriptionService.DeleteAsync(id);
                return NoContent();
            }catch(Exception ex)
            {
                _logger.LogError($"Error deleting subscription for user {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
