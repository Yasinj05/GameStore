using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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
            _ => (
                StatusCodes.Status500InternalServerError,
                "Server Error",
                "An unexpected internal error occurred."
            )
        };

        // برای خطاهای غیرمنتظره سرور لاگ کامل ثبت می‌کنیم، اما خطاهای عادی ۴۰۴ نیازی به لاگ بحرانی ندارند
        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "خطای بحرانی سرور: {Message}", exception.Message);
        }
        else
        {
            logger.LogWarning("خطای کلاینت/بیزینس: {Title} - {Detail}", title, detail);
        }

        ProblemDetails problemDetails;

        if (exception is ValidationException valEx)
        {
            var validationProblem = new HttpValidationProblemDetails(valEx.Errors)
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            problemDetails = validationProblem;
        }
        else
        {
            problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}