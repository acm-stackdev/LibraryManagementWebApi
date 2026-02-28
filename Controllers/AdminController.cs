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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        //Get all users
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
                .Select(u => new 
                {
                    u.Id,
                    u.Name,
                    u.Email,
                })
                .ToList();
            return Ok(users);
        }
        
        [HttpGet("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if(result.Succeeded)
            {
                return Ok("User deleted successfully");
            }
            return BadRequest(result.Errors);
        }
    }
}
