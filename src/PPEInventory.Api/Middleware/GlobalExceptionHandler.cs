using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PPEInventory.Application.Common.Exceptions;

namespace PPEInventory.Api.Middleware;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        switch (exception)
        {
            case ValidationException validationException:
                {
                    var errors = validationException.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(x => x.ErrorMessage)
                                .Distinct()
                                .ToArray());

                    problemDetails = new ValidationProblemDetails(errors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation error",
                        Detail = "One or more validation errors occurred.",
                        Instance = httpContext.Request.Path
                    };

                    break;
                }

            case NotFoundException:
                {
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Resource not found",
                        Detail = exception.Message,
                        Instance = httpContext.Request.Path
                    };

                    break;
                }

            case ConflictException:
                {
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Conflict",
                        Detail = exception.Message,
                        Instance = httpContext.Request.Path
                    };

                    break;
                }

            case UnauthorizedException:
                {
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = exception.Message,
                        Instance = httpContext.Request.Path
                    };

                    break;
                }

            case ConcurrencyException:
                {
                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Concurrency conflict",
                        Detail = exception.Message,
                        Instance = httpContext.Request.Path
                    };

                    break;
                }

            default:
                {
                    _logger.LogError(
                        exception,
                        "Unhandled exception processing {Method} {Path}",
                        httpContext.Request.Method,
                        httpContext.Request.Path);

                    problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Internal server error",
                        Detail = "An unexpected error occurred.",
                        Instance = httpContext.Request.Path
                    };

                    break;
                }
        }

        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode =
            problemDetails.Status
            ?? StatusCodes.Status500InternalServerError;

        httpContext.Response.ContentType =
            "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}