namespace BankingSystem.API.Middlewares
{
    public class ExceptionHandlerMiddleware 
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unhandled exception occured before reaching controller or while inner exception handling: {message}, {type}", ex.Message, ex.GetType().Name);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new { Message = $"An unhandled exception occured before reaching controller or while inner exception handling: {ex.Message}" };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
