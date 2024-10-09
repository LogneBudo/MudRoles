using Microsoft.Extensions.Primitives;
using MudRoles.Controllers;
using MudRoles.Infrastructure.Services;
using Nextended.Core.Extensions;

namespace MudRoles.Infrastructure.MiddleWare
{
    /// <summary>
    /// Middleware to validate API keys for incoming HTTP requests.
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
        public ApiKeyMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Invokes the middleware to validate the API key.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request path requires API key validation
            if (context.Request.Path.StartsWithSegments("/api") && !context.Request.Path.StartsWithSegments("/api/configuration"))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var apiKeyService = scope.ServiceProvider.GetRequiredService<IApiKeyService>();

                    if (!context.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey) || StringValues.IsNullOrEmpty(extractedApiKey))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("API Key was not provided.");
                        return;
                    }

                    if (!await apiKeyService.ValidateApiKeyAsync(extractedApiKey.ToString()))
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
