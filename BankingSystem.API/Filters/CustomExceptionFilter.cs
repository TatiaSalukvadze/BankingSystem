using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.API.Filters
{
    public class CustomExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            var error = new 
            {
                Description = exception.Message,
                ErrorType = exception.GetType().Name,
                exception.Source,
            };

            _logger.LogError("Exception Filter Caught Exception: {ExceptionMessage},  Controller: {Controller}, Action: {Action}",
            exception.Message, context.RouteData.Values["Controller"], context.RouteData.Values["Action"]);
            
            context.Result = new BadRequestObjectResult(error);
            context.ExceptionHandled = true;
            await Task.CompletedTask;
        }
    }
}
