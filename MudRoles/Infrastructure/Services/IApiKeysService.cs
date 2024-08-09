namespace MudRoles.Infrastructure.Services
{
    public interface IApiKeyService
    {
        Task<bool> ValidateApiKeyAsync(string apiKey);
        // Add other methods as needed
    }
}
