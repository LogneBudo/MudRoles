using Microsoft.Extensions.Caching.Memory;

namespace MudRoles.Infrastructure.MiddleWare
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

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
