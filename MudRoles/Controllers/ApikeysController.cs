using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudRoles.Data.ApiData;
using System.Security.Claims;
using MudRoles.Data;
using MudRoles.Infrastructure.Api;

namespace MudRoles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApikeysController(ApiKeyDbContext context, ApplicationDbContext applicationContext) : ControllerBase
    {
        private readonly ApiKeyDbContext _context = context;
        private readonly ApplicationDbContext _applicationContext = applicationContext;

        // GET: api/ApiKeys
        [HttpGet]
        [Authorize(Roles = "Admin,User")] // Both Admin and User roles can access this endpoint
        public async Task<ActionResult<IEnumerable<ApiKey>>> GetApiKeys()
        {
            var apiKeys = await _context.ApiKeys.ToListAsync();
            return Ok(apiKeys);
        }

        // GET: api/ApiKeys/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")] // Both Admin and User roles can access this endpoint
        public async Task<ActionResult<ApiKey>> GetApiKey(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);

            if (apiKey == null)
            {
                return NotFound();
            }

            return apiKey;
        }

        // POST: api/ApiKeys
        [HttpPost]
        [Authorize(Roles = "Admin,User")] // Both Admin and User roles can request new API keys
        public async Task<ActionResult<ApiKey>> PostApiKey()
        {
            // Ensure the user is authenticated
            if (User.Identity?.IsAuthenticated != true)
            {
                return Unauthorized();
            }
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the ApplicationUser from the database
            var user = await _applicationContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var apiKey = new ApiKey
            {
                Name = user.UserName, // Default name
                KeyPrefix = "API-KEY",
                Key = ApiKeyGenerator.GenerateApiKey(),
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddYears(1), // Example: 1 year expiration
                UserId = user.Id, // Assuming the owner is the current user
                Status = KeyStatus.Active,
                Scopes = new List<Scope> { new Scope {  ScopeVerb = "GET", EndPoint = "/api/apikeys" }, new Scope { ScopeVerb = "Post", EndPoint = "/api/apikeys" } }
            };
            _context.ApiKeys.Add(apiKey);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApiKey), new { id = apiKey.Id }, apiKey);
        }

        // PUT: api/ApiKeys/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin role can access this endpoint
        public async Task<IActionResult> PutApiKey(int id, ApiKey apiKey)
        {
            if (id != apiKey.Id)
            {
                return BadRequest();
            }

            _context.Entry(apiKey).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiKeyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ApiKeys/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin role can access this endpoint
        public async Task<IActionResult> DeleteApiKey(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return NotFound();
            }

            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApiKeyExists(int id)
        {
            return _context.ApiKeys.Any(e => e.Id == id);
        }
    }
}
