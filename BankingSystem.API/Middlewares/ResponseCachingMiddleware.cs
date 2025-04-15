using Microsoft.Extensions.Caching.Memory;

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
            var routeData = context.GetRouteData();
            if (routeData is null || routeData?.Values.Count == 0 ||
                !routeData.Values.TryGetValue("Controller", out var controllerObj) ||
                !routeData.Values.TryGetValue("Action", out var actionObj)
                || context.Request.Method != HttpMethods.Get || string.Equals(controllerObj?.ToString(), "Auth")) 
            {
                await _next(context);
                return;
            }

            var action = actionObj.ToString();

            _logger.LogInformation("Response of the sent request might be in MemoryCache!");
            string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";
            string cacheKey = $"{action}{queryString}";
            string cacheKeyStatusCode = $"{cacheKey}StatusCode";

            if (_memoryCache.TryGetValue(cacheKey, out string response) && response != ""
                && _memoryCache.TryGetValue(cacheKeyStatusCode, out int retrievedStatusCode))
            {
                _logger.LogInformation("Getting request response from MemoryCache!");
                context.Response.StatusCode = retrievedStatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response);
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
                _memoryCache.Set(cacheKeyStatusCode, statusCode, cacheOptions);

                context.Response.Body = originalBodyStream;
                await context.Response.Body.WriteAsync(memoryStream.ToArray());
            }
        }
    }
}
