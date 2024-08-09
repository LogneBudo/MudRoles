using MudRoles.Controllers;
using MudRoles.Infrastructure.Services;

namespace MudRoles.Infrastructure.MiddleWare
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public ApiKeyMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request path requires API key validation
            if (context.Request.Path.StartsWithSegments("/api") && !context.Request.Path.StartsWithSegments("/api/configuration"))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var apiKeyService = scope.ServiceProvider.GetRequiredService<IApiKeyService>();

                    if (!context.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("API Key was not provided.");
                        return;
                    }

                    if (string.IsNullOrEmpty(extractedApiKey) || !await apiKeyService.ValidateApiKeyAsync(extractedApiKey))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized client.");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
