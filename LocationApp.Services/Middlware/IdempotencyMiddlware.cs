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

                var originalBodyStream = context.Response.Body;

                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var statusCode = context.Response.StatusCode;
                _cache.Set(idempotencyKey, (responseText, statusCode), TimeSpan.FromMinutes(5));

                await responseBody.CopyToAsync(originalBodyStream);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
