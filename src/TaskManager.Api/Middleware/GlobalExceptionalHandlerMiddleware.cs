using FluentValidation;
using System.Net;
using System.Text.Json;
using TaskManager.Application.Common.Models;

namespace TaskManager.Api.Middleware;
public class GlobalExceptionalHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionalHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionalHandlerMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionalHandlerMiddleware> logger, 
        IHostEnvironment env)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Uno o mas errores de validacoin ocurrieron",
                Errors = validationException.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList(),
                TraceId = context.TraceIdentifier
            },

            KeyNotFoundException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,  // 404
                Message = exception.Message,
                TraceId = context.TraceIdentifier
            },

            UnauthorizedAccessException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,  // 401
                Message = "No tienes permisos para realizar esta acción",
                TraceId = context.TraceIdentifier
            },

            ArgumentException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,  // 400
                Message = exception.Message,
                TraceId = context.TraceIdentifier
            },

            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,  // 500
                Message = _env.IsDevelopment()
                    ? exception.Message
                    : "Ocurrió un error interno en el servidor",
                StackTrace = _env.IsDevelopment()
                    ? exception.StackTrace
                    : null,  // Solo en desarrollo
                TraceId = context.TraceIdentifier
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // camelCase
            WriteIndented = true  // JSON formateado
        });

        await context.Response.WriteAsync(jsonResponse);

    }
}

