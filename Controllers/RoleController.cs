using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        //Get all roles
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                return NotFound("Role not found");
            }
            return Ok(role);
        }

        //Create a new role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if(await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("Role already exists");
            }
            
            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);
            if(result.Succeeded)
            {
                return Ok("Role created successfully");
            }
            return BadRequest(result.Errors);
        }
        
        //Update an existing role
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if(role == null)
            {
                return NotFound("Role not found");
            }

            role.Name = model.NewRoleName;
            var result = await _roleManager.UpdateAsync(role);
            
            if(result.Succeeded)
            {
                return Ok("Role updated successfully");
            }
            return BadRequest(result.Errors);
        }
        
        //Delete a role
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role =await _roleManager.FindByIdAsync(roleId);
            if(role == null) return NotFound("Role not found");
            
            var result = await _roleManager.DeleteAsync(role);
            if(result.Succeeded)
            {
                return Ok("Role deleted successfully");
            }
            return BadRequest(result.Errors);
        }

        //Assign a role to a user
        [HttpPost("assign-role-to-user")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if(user == null) return NotFound("User not found");
            
            if(!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                return NotFound("Role not found");
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if(result.Succeeded)
            {
                return Ok($"Role {model.RoleName} assigned successfully to user {user.Email}");
            }
            return BadRequest(result.Errors);
        }
    }
}