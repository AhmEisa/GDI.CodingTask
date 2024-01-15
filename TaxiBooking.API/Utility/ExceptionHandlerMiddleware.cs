using FluentValidation;
using System.Net;
using System.Text.Json;

namespace TaxiBooking.API.Utility
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"Validation Failed: {ex.Message}");

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message ?? string.Join(',', ex.Errors),
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "An error occurred while processing your request."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
