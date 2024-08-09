using Microsoft.Extensions.Caching.Memory;

namespace MudRoles.Infrastructure.MiddleWare
{
    /// <summary>
    /// Middleware to enforce rate limiting on incoming HTTP requests.
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="memoryCache">The memory cache to store rate limiting data.</param>
        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Invokes the middleware to enforce rate limiting.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request path starts with /api
            if (context.Request.Path.StartsWithSegments("/api") && !context.Request.Path.StartsWithSegments("/api/configuration"))
            {
                var apiKey = context.Request.Headers["ApiKey"].ToString();

                if (string.IsNullOrEmpty(apiKey) || !await AllowRequestAsync(apiKey))
                {
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                    return;
                }
            }

            await _next(context);
        }

        /// <summary>
        /// Determines whether the request is allowed based on the rate limit.
        /// </summary>
        /// <param name="apiKey">The API key of the client making the request.</param>
        /// <returns>A task that represents whether the request is allowed.</returns>
        private Task<bool> AllowRequestAsync(string apiKey)
        {
            // Example rate limiting logic using in-memory storage
            var cacheKey = $"RateLimit_{apiKey}";
            var requestCount = _memoryCache.Get<int?>(cacheKey) ?? 0;

            if (requestCount >= 100) // Example limit: 100 requests
            {
                return Task.FromResult(false);
            }

            _memoryCache.Set(cacheKey, requestCount + 1, TimeSpan.FromMinutes(1));
            return Task.FromResult(true);
        }
    }
}
