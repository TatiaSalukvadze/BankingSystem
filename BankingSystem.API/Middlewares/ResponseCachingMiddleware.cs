using Azure;
using BankingSystem.API.Controllers.OnlineBank;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Text;
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
            //var originalBodyStream = context.Response.Body;

            //// Create new memory stream for reading the response; Response body streams are write-only, therefore memory stream is needed here to read
            //await using var memoryStream = new MemoryStream();
            //context.Response.Body = memoryStream;

            //// Call the next middleware
            //await _next(context);

            //// Set stream pointer position to 0 before reading
            //memoryStream.Seek(0, SeekOrigin.Begin);

            //// Read the body from the stream
            //var responseBodyText = await new StreamReader(memoryStream).ReadToEndAsync();

            //// Reset the position to 0 after reading
            //memoryStream.Seek(0, SeekOrigin.Begin);

            //// Do this last, that way you can ensure that the end results end up in the response.
            //// (This resulting response may come either from the redirected route or other special routes if you have any redirection/re-execution involved in the middleware.)
            //// This is very necessary. ASP.NET doesn't seem to like presenting the contents from the memory stream.
            //// Therefore, the original stream provided by the ASP.NET Core engine needs to be swapped back.
            //// Then write back from the previous memory stream to this original stream.
            //// (The content is written in the memory stream at this point; it's just that the ASP.NET engine refuses to present the contents from the memory stream.)
            //context.Response.Body = originalBodyStream;
            //await context.Response.Body.WriteAsync(memoryStream.ToArray());

            var controller = context.GetRouteData().Values["Controller"].ToString();
            var action = context.GetRouteData().Values["Action"].ToString();
            bool fromMemory = false;
            if (context.Request.Method == HttpMethods.Get && controller != "Auth")//controller == "UserBanking" && (action == "SeeAccounts" || action == "SeeCards"))
            {
                _logger.LogInformation("Response of the sent request might be in MemoryCache!");
                if (_memoryCache.TryGetValue(action, out var response) && response != "")
                {
                    _logger.LogInformation("Getting request response from MemoryCache!");
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/json";
                    var jsonResponse = JsonSerializer.Deserialize<object>(response.ToString());
                    await context.Response.WriteAsJsonAsync(jsonResponse);
                    //var jsonResponse = JsonSerializer.Serialize(response);
                    //await using var memoryStreamForCache = new MemoryStream();
                    //memoryStreamForCache.Seek(0, SeekOrigin.Begin);
                    //await new StreamWriter(memoryStreamForCache).WriteLineAsync(jsonResponse);
                    //memoryStreamForCache.Seek(0, SeekOrigin.Begin);
                    //context.Response.Body = memoryStreamForCache;
                    //context.Response.StatusCode = 200;
                    //fromMemory = true;
                    //return;
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

                    var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(15));
                    _memoryCache.Set(action, serviceResponse, cacheOptions);

                    context.Response.Body = originalBodyStream;
                    await context.Response.Body.WriteAsync(memoryStream.ToArray());
                    //await _next(context);
                    //if (!fromMemory)
                    //{
                    //    string responseBody = new StreamReader(context.Request.Body).ReadToEnd(); //responseBody is ""
                    //    context.Request.Body.Position = 0;
                    //    var serviceResponse = JsonSerializer.Deserialize<object>(context.Response.Body);
                    //    var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                    //    _memoryCache.Set(action, serviceResponse, cacheOptions);


                    //}
                    //return;
                }
            }
            else
            {

                await _next(context);
            }
            

        }
    }
}
