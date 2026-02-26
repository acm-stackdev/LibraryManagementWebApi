using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BackendApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        } 

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(string.IsNullOrEmpty(model.Name)){
                return BadRequest("Name is required for registration.");
            }
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var verificationLink = Url.Action(
                    "VerifyEmail",
                    "Account",
                    new {userId = user.Id, token = token },
                    Request.Scheme
                );

                _emailService.SendEmail(
                    user.Email,
                    "Email Verification",
                    $@"
    <html>
        <body style=""font-family: Arial, sans-serif; line-height:1.6; color: #333;"">
            <h2 style=""color: #2C3E50;"">Welcome to Library Management System!</h2>
            <p>Hi {user.Name},</p>
            <p>Thank you for registering. Please click the button below to verify your email address:</p>
            <p style=""text-align: center;"">
                <a href=""{verificationLink}"" 
                   style=""background-color: #3498DB; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">
                   Verify Email
                </a>
            </p>
            <hr>
            <p style=""font-size: 12px; color: #888;"">This is an automated email from Library Management System.</p>
        </body>
    </html>"
                );

                return Ok("Registration successful. Please check your email for verification.");
            }

            return BadRequest(result.Errors);
        }  

         [HttpGet("verify-email")]
         public async Task<IActionResult> VerifyEmail(string userId, string token)
         {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
                return NotFound("User not found");
            
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded)
            {
                return Ok("Email verified successfully.");
            }
            return BadRequest("Failed to verify email.");
         }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );
            if(result.Succeeded)
            {
              var user = await _userManager.FindByEmailAsync(model.Email);

              if(!user.EmailConfirmed)
              {
                return Unauthorized("Please confirm your email first.");
              }   

              var roles = await _userManager.GetRolesAsync(user);
              var token = GenerateJwtToken(user, roles);
              
              return Ok(new { Token = token });
            }
            return Unauthorized("Invalid email or password.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful.");
        }

        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
         var claims = new List<Claim>
         {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
         };

         foreach (var role in roles)
         {
            claims.Add(new Claim(ClaimTypes.Role, role));
         }   

         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok("User deleted successfully.");

            return BadRequest(result.Errors);
        }
    }
}