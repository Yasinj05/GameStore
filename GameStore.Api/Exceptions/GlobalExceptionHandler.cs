using GameStore.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                notFoundEx.Message
            ),
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                validationEx.Message
            ),
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Concurrency Conflict",
                "این رکورد توسط درخواست دیگری تغییر کرده است. لطفاً آخرین نسخه را دریافت و دوباره امتحان کنید."
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Server Error",
                "An unexpected internal error occurred."
            )
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "خطای بحرانی سرور: {Message}", exception.Message);
        }
        else
        {
            logger.LogWarning("خطای کلاینت/بیزینس: {Title} - {Detail}", title, detail);
        }

        var problemDetails = exception is ValidationException valEx
            ? new HttpValidationProblemDetails(valEx.Errors)
            : new ProblemDetails();

        problemDetails.Status = statusCode;
        problemDetails.Title = title;
        problemDetails.Detail = detail;
        problemDetails.Instance = httpContext.Request.Path;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}