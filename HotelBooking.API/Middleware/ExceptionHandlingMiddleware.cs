using HotelBooking.Application.Common.Models;
using System.Text.Json;

namespace HotelBooking.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
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
