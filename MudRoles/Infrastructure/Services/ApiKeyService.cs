using Microsoft.EntityFrameworkCore;
using MudRoles.Data;

namespace MudRoles.Infrastructure.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly ApiKeyDbContext _context;

        public ApiKeyService(ApiKeyDbContext context)
        {
            _context = context;
        }

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
