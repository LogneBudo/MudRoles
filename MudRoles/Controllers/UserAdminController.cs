using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MudRoles.Data.ApiData;
using MudRoles.Data;
using Microsoft.EntityFrameworkCore;
using MudRoles.Client;
using Microsoft.AspNetCore.Identity;
using MudRoles.Client.Components;
using MudRoles.Infrastructure.Converter;

namespace MudRoles.Controllers
{
    /// <summary>
    /// Controller for managing users and roles.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserAdminController(ApiKeyDbContext context, ApplicationDbContext applicationContext, UserManager<ApplicationUser> um, RoleManager<IdentityRole> roleManager) : ControllerBase
    {
        private readonly ApiKeyDbContext _context = context;
        private readonly ApplicationDbContext _applicationContext = applicationContext;
        private readonly UserManager<ApplicationUser> _uM = um;
        private readonly RoleManager<IdentityRole> _rM = roleManager;

        /// <summary>
        /// Retrieves a list of users along with their roles.
        /// </summary>
        /// <returns>A list of UserInfo objects.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetUsers()
        {
            // Ensure the user is authenticated
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }

            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the ApplicationUser from the database
            var users = await _applicationContext.Users.ToListAsync();
            List<UserInfo> usersInfo = new List<UserInfo>();
            foreach (var user in users)
            {
                var roles = await _uM.GetRolesAsync(user);
                usersInfo.Add(new UserInfo
                {
                    UserId = user.Id!,
                    Email = user.Email!,
                    Roles = RoleConverter.ConvertToRoleItems(roles)
                });
            }
            if (users == null)
            {
                return NotFound("No User found");
            }

            return Ok(usersInfo);
        }

        /// <summary>
        /// Retrieves a list of roles.
        /// </summary>
        /// <returns>A list of RoleItem objects.</returns>
        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RoleItem>>> GetRoles()
        {
            var roles = await _rM.Roles.ToListAsync();
            List<RoleItem> roleItems = new List<RoleItem>();
            foreach (var role in roles)
            {
                roleItems.Add(new RoleItem
                {
                    Id = role.Id,
                    RoleName = role.Name ?? "",
                    IsAssigned = false,
                    NormalizedName = role.NormalizedName ?? ""
                });
            }
            return Ok(roleItems);
        }

        /// <summary>
        /// Updates the roles assigned to a user.
        /// </summary>
        /// <param name="userInfo">The user information including roles to be updated.</param>
        /// <returns>Action result indicating success or failure.</returns>
        [HttpPost("updateUserRoles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UserInfo userInfo)
        {
            var user = await _uM.FindByIdAsync(userInfo.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var currentRoles = await _uM.GetRolesAsync(user);
            var rolesToAdd = userInfo.Roles.Where(role => role.IsAssigned && !currentRoles.Contains(role.RoleName)).Select(role => role.RoleName).ToList();
            var rolesToRemove = currentRoles.Where(role => !userInfo.Roles.Any(r => r.RoleName == role && r.IsAssigned)).ToList();

            var addResult = await _uM.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                return BadRequest("Failed to add roles");
            }

            var removeResult = await _uM.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                return BadRequest("Failed to remove roles");
            }

            return Ok("User roles updated successfully");
        }

        /// <summary>
        /// Deletes a role by its name.
        /// </summary>
        /// <param name="roleName">The name of the role to be deleted.</param>
        /// <returns>Action result indicating success or failure.</returns>
        [HttpDelete("{roleName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _rM.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            // Validate that the role is not "User" or "Admin"
            if (role.Name == "User" || role.Name == "Admin")
            {
                return BadRequest("Cannot delete 'User' or 'Admin' roles");
            }

            var result = await _rM.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to delete role");
            }

            return Ok("Role deleted successfully");
        }

        /// <summary>
        /// Adds a new role.
        /// </summary>
        /// <param name="roleItem">The role information to be added.</param>
        /// <returns>The created RoleItem object.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoleItem>> AddRole([FromBody] RoleItem roleItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (roleItem == null || string.IsNullOrWhiteSpace(roleItem.RoleName))
            {
                return BadRequest("Invalid role data");
            }

            // Check if the role already exists
            var existingRole = await _rM.FindByNameAsync(roleItem.RoleName);
            if (existingRole != null)
            {
                return BadRequest("Role already exists");
            }

            // Create the new role
            var role = new IdentityRole
            {
                Name = roleItem.RoleName,
                NormalizedName = roleItem.NormalizedName,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _rM.CreateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to add role");
            }

            roleItem.Id = role.Id; // Set the Id of the newly created role

            return CreatedAtAction(nameof(AddRole), new { id = roleItem.Id }, roleItem);
        }
    }
}
