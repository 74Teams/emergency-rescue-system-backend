using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using RescueSystem.Application.Common.Response;

namespace RescueSystem.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "Internal Server Error";
            object? errors = null;

            if (exception is ValidationException validationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Validation Failed";
                errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            }
            else
            {
                
                var typeName = exception.GetType().FullName ?? string.Empty;

                switch (typeName)
                {
                    case "RescueSystem.Application.Common.Exception.BadRequestException":
                        statusCode = (int)HttpStatusCode.BadRequest;
                        message = exception.Message;
                        break;
                    case "RescueSystem.Application.Common.Exception.NotFoundException":
                        statusCode = (int)HttpStatusCode.NotFound;
                        message = exception.Message;
                        break;
                    case "RescueSystem.Application.Common.Exception.UnauthorizedException":
                        statusCode = (int)HttpStatusCode.Unauthorized;
                        message = exception.Message;
                        break;
                    case "RescueSystem.Application.Common.Exception.InternalServerErrorException":
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        message = exception.Message;
                        break;
                    default:
                        var appEx = new ApplicationException("Internal Server Error");
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        message = _env.IsDevelopment() ? exception.Message : appEx.Message;
                        break;
                }
            }

            var response = ApiResponse<object>.ErrorResponse(message, statusCode, errors);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
