using LocationApp.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace LocationApp.Services.Middlware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRepositoryManager repositoryManager)
        {
            try
            {
                // Bypass the API key check for some routes
                if (context.Request.Path.StartsWithSegments("/searchHub") ||
                    context.Request.Path.StartsWithSegments("/api/authentication") ||
                    context.Request.Path.StartsWithSegments("/api/cron"))
                {
                    await _next(context);
                    return;
                }

                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var base64Credentials = context.Request.Headers["Authorization"].ToString().Substring("Basic ".Length).Trim();
                    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials));
                    var apiKey = credentials.Split(':')[0];

                    var user = await repositoryManager.User.GetUser(apiKey);

                    if (user != null)
                    {
                        // Add the authenticated user to HttpContext for further use in the controller
                        context.Items["User"] = user;
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid API Key.");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("API Key is missing.");
                    return;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Internal Server Error: {ex.Message}");
            }
        }
    }
}
