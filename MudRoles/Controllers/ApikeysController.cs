﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudRoles.Data.ApiData;
using System.Security.Claims;
using MudRoles.Data;
using MudRoles.Infrastructure.Api;
using ApiKey = MudRoles.Data.ApiData.ApiKey;
using MudRoles.Client.Components;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

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
        {// Ensure the user is authenticated
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
            var apiKeys = await _context.ApiKeys.Where(x=> x.UserId == user.Id).ToListAsync();
            return Ok(apiKeys);
        }
        // GET: api/Scopes
        [HttpGet("Scopes")]
        // Both Admin and User roles can access this endpoint
        public async Task<ActionResult<IEnumerable<Scope>>> GetScopes()
        {
            var scocpes = ScopeFetcher.GetAllRoutes();
            await Task.Yield();
            return Ok(scocpes);
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
        public async Task<ActionResult<ApiKey>> PostApiKey(KeyInputModel formData)
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
                Name = formData.ApiKeyName ?? "DefaultName", // Default name
                KeyPrefix = "Akee-CP",
                Key = ApiKeyGenerator.GenerateApiKey(),
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddYears(1), // Example: 1 year expiration
                UserId = user.Id, // Assuming the owner is the current user
                Status = KeyStatus.Active,
                ScopesJson = JsonSerializer.Serialize(formData.Scopes.Where(x=> x.IsChecked))
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
        [Authorize(Roles = "Admin,User")] // Only Admin role can access this endpoint
        public async Task<IActionResult> DeleteApiKey(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return NotFound("API key not found");
            }

            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private async Task<ApiKey?> GetApiKeyFromFullKey(string fullKey)
        {
            var splitKey = fullKey.Split('-');
            if (splitKey.Length != 2)
            {
                return null;
            }
            var keyPrefix = splitKey[0];
            var key = splitKey[1];

            return await _context.ApiKeys.FirstOrDefaultAsync(apiKey => apiKey.KeyPrefix == keyPrefix && apiKey.Key == key);
        }


        // PATCH: api/ApiKeys/Renew/{id}
        [HttpPatch("Renew/{id}")]
        [Authorize(Roles = "Admin,User")] // Both Admin and User roles can renew API keys
        public async Task<IActionResult> RenewApiKey(int id)
        {
            var apiKey = await _context.ApiKeys.FindAsync(id);
            if (apiKey == null)
            {
                return NotFound("API key not found");
            }

            var daysUntilExpiration = (apiKey.ExpirationDate - DateTime.UtcNow).TotalDays;
            if (daysUntilExpiration > 30)
            {
                return BadRequest("Your key is still valid. Come back 30 days before expiry to renew it.");
            }

            apiKey.ExpirationDate = DateTime.UtcNow.AddYears(1); // Renew for another year
            apiKey.Status = KeyStatus.Active;
            _context.Entry(apiKey).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(apiKey);
        }


        private bool ApiKeyExists(int id)
        {
            return _context.ApiKeys.Any(e => e.Id == id);
        }
    }
}
