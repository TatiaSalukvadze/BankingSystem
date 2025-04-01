using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace BankingSystem.API.Middlewares
{
    public class ResponseCachingMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        public ResponseCachingMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next, IMemoryCache memoryCache)
        {
            _logger = logger;
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var controller = context.GetRouteData().Values["Controller"].ToString();
            var action = context.GetRouteData().Values["Action"].ToString();
            bool fromMemory = false;

            if (context.Request.Method == HttpMethods.Get && controller != "Auth")
            {
                _logger.LogInformation("Response of the sent request might be in MemoryCache!");
                string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";
                var cacheKey = $"{action}{queryString}";

                if (_memoryCache.TryGetValue(cacheKey, out var response) && response != ""
                    && _memoryCache.TryGetValue(cacheKey + "StatusCode", out int retreivedStatusCode))
                {
                    _logger.LogInformation("Getting request response from MemoryCache!");
                    context.Response.StatusCode = retreivedStatusCode;//StatusCodes.Status200OK;
                    context.Response.ContentType = "application/json";
                    var jsonResponse = JsonSerializer.Deserialize<object>(response.ToString());
                    await context.Response.WriteAsJsonAsync(jsonResponse);
                }
                else
                {
                    _logger.LogInformation("Getting original response from db and saving it in MemoryCache!");
                    var originalBodyStream = context.Response.Body;

                    await using var memoryStream = new MemoryStream();
                    context.Response.Body = memoryStream;

                    await _next(context);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var serviceResponse = await new StreamReader(memoryStream).ReadToEndAsync();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var statusCode = context.Response.StatusCode;

                    var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(15));
                    _memoryCache.Set(cacheKey, serviceResponse, cacheOptions);
                    _memoryCache.Set(cacheKey + "StatusCode", statusCode, cacheOptions);

                    context.Response.Body = originalBodyStream;
                    await context.Response.Body.WriteAsync(memoryStream.ToArray());
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
