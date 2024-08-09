using Microsoft.EntityFrameworkCore;
using MudRoles.Data;

namespace MudRoles.Infrastructure.Services
{
    /// <summary>
    /// Service to handle API key validation.
    /// </summary>
    public class ApiKeyService : IApiKeyService
    {
        private readonly ApiKeyDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyService"/> class.
        /// </summary>
        /// <param name="context">The database context to access API keys.</param>
        public ApiKeyService(ApiKeyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Validates the provided API key asynchronously.
        /// </summary>
        /// <param name="apiKey">The API key to validate.</param>
        /// <returns>A task that represents whether the API key is valid.</returns>
        public async Task<bool> ValidateApiKeyAsync(string apiKey)
        {
            // Split the provided API key into prefix and key parts
            var parts = apiKey.Split('-', 3); // Split into 3 parts to handle the prefix correctly
            if (parts.Length < 3)
            {
                return false;
            }

            var keyPrefix = $"{parts[0]}-{parts[1]}";
            var key = parts[2];

            // Validate the concatenated key
            var isValid = await _context.ApiKeys.AnyAsync(k => k.KeyPrefix == keyPrefix && k.Key == key);
            return isValid;
        }
    }
}
