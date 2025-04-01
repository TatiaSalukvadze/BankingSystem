using BankingSystem.Contracts.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankingSystem.API.Filters
{
    public class ResponseFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is null || context.Result is not ObjectResult)
            {
                context.Result = new ObjectResult(new { Message = "Result was not correctly returned!" })
                { StatusCode = 404 };
            }
            var objectResult = context.Result as ObjectResult;
            if (objectResult.Value is SimpleResponse simpleResponse)
            {
                objectResult.Value = new { message = simpleResponse.Message };
                objectResult.StatusCode = simpleResponse.StatusCode;
            }
            else if (objectResult.Value.GetType().IsGenericType && objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(Response<>))
            {
                Type resultType = objectResult.Value.GetType();
                var response = objectResult.Value;

                var success = resultType.GetProperty("Success")?.GetValue(response);
                var statusCode = resultType.GetProperty("StatusCode")?.GetValue(response);

                var messageProperty = resultType.GetProperty("Message");
                var dataProperty = resultType.GetProperty("Data");

                if (success is null || statusCode is null ||
                    messageProperty is null || dataProperty is null)
                {
                    context.Result = new ObjectResult(new { Message = "Result values were not retreived!" })
                    { StatusCode = 500 };
                }
                else
                {
                    var message = messageProperty.GetValue(response);
                    var data = dataProperty.GetValue(response);
                    objectResult.Value = (bool)success ? new { message, data } : new { message };
                    objectResult.StatusCode = (int)statusCode;
                }
            }
            await next();
        }
    }
}
