using LocationApp.Model.Core;
using LocationApp.Repository;
using Newtonsoft.Json;
using System.Text;

namespace LocationApp.Account.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RequestResponseLoggingMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var originalResponseBodyStream = context.Response.Body;

            try
            {
                var requestBody = await GetRequestBodyAsync(request);
                var requestHeaders = JsonConvert.SerializeObject(request.Headers);

                // Temporary response body stream
                var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Continue processing the request
                await _next(context);

                // Log the response
                var responseBody = await GetResponseBodyAsync(responseBodyStream);
                var responseHeaders = JsonConvert.SerializeObject(context.Response.Headers);

                var log = new RequestResponseLog
                {
                    EndpointUrl = request.Path,
                    RequestMethod = request.Method,
                    RequestHeaders = requestHeaders,
                    RequestBody = requestBody,
                    ResponseHeaders = responseHeaders,
                    ResponseBody = responseBody,
                    ResponseStatusCode = context.Response.StatusCode
                };

                // Write the log to the database
                await LogToDatabaseAsync(log);

                // Reset response stream position and copy data back to the original stream
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalResponseBodyStream);
            }
            catch (Exception ex)
            {
                // Log the exception (optional: you can log this to your database or logging system)
                Console.WriteLine($"Middleware Exception: {ex.Message}");

                // Re-throw the exception to let the pipeline handle it
                throw;
            }
            finally
            {
                // Ensure the original response stream is restored
                context.Response.Body = originalResponseBodyStream;
            }
        }

        private async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            if (request.ContentLength > 0)
            {
                request.EnableBuffering();

                using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var body = await reader.ReadToEndAsync();
                    request.Body.Position = 0; // Reset the position for further reading
                    return body;
                }
            }
            return string.Empty;
        }

        private async Task<string> GetResponseBodyAsync(Stream responseBodyStream)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task LogToDatabaseAsync(RequestResponseLog log)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();
                await repositoryManager.Log.CreateLog(log);
            }
        }
    }
}
