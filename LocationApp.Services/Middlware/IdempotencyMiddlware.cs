using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace LocationApp.Services.Middlware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public IdempotencyMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
            {
                if (_cache.TryGetValue(idempotencyKey, out string previousResponse))
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(previousResponse);
                    return;
                }

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _cache.Set(idempotencyKey, (responseText, context.Response.StatusCode), TimeSpan.FromMinutes(1));
            }
            else
            {
                await _next(context);
            }
        }
    }
}
