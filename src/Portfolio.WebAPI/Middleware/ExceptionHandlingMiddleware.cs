using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Portfolio.WebAPI.Middleware;

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
            _logger.LogError(ex, "An unhandled exception occurred during request execution.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        var title = "An error occurred while processing your request.";
        var detail = exception.Message;

        if (exception is KeyNotFoundException)
        {
            statusCode = HttpStatusCode.NotFound;
            title = "The specified resource was not found.";
        }
        else if (exception is FluentValidation.ValidationException valEx)
        {
            statusCode = HttpStatusCode.BadRequest;
            title = "One or more validation errors occurred.";
            
            var problemDetails = new ValidationProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = "Please refer to the errors property for details."
            };

            foreach (var error in valEx.Errors)
            {
                problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
            }

            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }

        var responseProblemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(responseProblemDetails));
    }
}
