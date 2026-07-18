using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuartzScheduler.API.Exceptions;
using QuartzScheduler.Shared.DTOs.Common;
using System.Text.Json;

namespace QuartzScheduler.API.Middleware;

// status code, and returns a consistent JSON body. Controllers need no try/catch.
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message, errors) = ex switch
        {
            // FluentValidation -> 400 with per-field messages
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.",
                validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())
                    as IDictionary<string, string[]>
            ),

            NotFoundException => (StatusCodes.Status404NotFound, ex.Message, null),
            ConflictException => (StatusCodes.Status409Conflict, ex.Message, null),
            BadRequestException => (StatusCodes.Status400BadRequest, ex.Message, null),
            ForbiddenException => (StatusCodes.Status403Forbidden, ex.Message, null),

            DbUpdateException dbEx when IsUniqueConstraintViolation(dbEx) => (
                StatusCodes.Status409Conflict,
                "A record with these details already exists.",
                (IDictionary<string, string[]>?)null
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                (IDictionary<string, string[]>?)null
            )
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
        }
        else
        {
            logger.LogWarning("{Exception} on {Path}: {Message}",
                ex.GetType().Name, context.Request.Path, ex.Message);
        }

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Errors = errors
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    // SQL Server unique-constraint error numbers
    private static bool IsUniqueConstraintViolation(DbUpdateException ex) =>
        ex.InnerException?.Message.Contains("duplicate key") == true
        || ex.InnerException?.Message.Contains("2601") == true
        || ex.InnerException?.Message.Contains("2627") == true;
}