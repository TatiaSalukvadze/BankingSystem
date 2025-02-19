using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingSystem.API.Filters
{
    public class CustomExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            var error = new 
            {
                Description = exception.Message,
                ErrorType = exception.GetType().Name,
            };
           
            context.Result = new BadRequestObjectResult(error);
            context.ExceptionHandled = true;
            await Task.CompletedTask;
        }
    }
}
