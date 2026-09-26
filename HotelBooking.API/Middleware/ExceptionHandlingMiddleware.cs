using HotelBooking.Application.Common.Models;
using System.Text.Json;

namespace HotelBooking.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. Method: {Method}, Path: {Path}",
                    context.Request.Method,
                    context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,

                InvalidOperationException => StatusCodes.Status409Conflict,

                _ => StatusCodes.Status500InternalServerError
            };

            var response = ApiResponse<object>.Fail(
                exception.Message
            );

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}
