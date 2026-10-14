using TAO.Api.Contracts;
using TAO.SharedKernel.Results;

namespace TAO.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResult<T>(
        this Result<T> result,
        HttpContext httpContext,
        string successMessage = "Request completed successfully.")
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse<T>.Succeeded(
                result.Value!,
                successMessage,
                httpContext.TraceIdentifier);

            return Results.Ok(response);
        }

        var error = result.Error!;

        var responseOnFailure = ApiResponse<T>.Failed(
            error.Message,
            [
                new ApiError(
                    error.Code,
                    error.Message)
            ],
            httpContext.TraceIdentifier);

        var statusCode = GetStatusCode(error.Code);

        return Results.Json(
            responseOnFailure,
            statusCode: statusCode);
    }


    public static IResult ToCreatedApiResult<T>(
    this Result<T> result,
    HttpContext httpContext,
    Func<T, string> locationFactory,
    string successMessage = "Resource created successfully.")
    {
        if (!result.IsSuccess)
        {
            return result.ToApiResult(httpContext);
        }

        var response = ApiResponse<T>.Succeeded(
            result.Value!,
            successMessage,
            httpContext.TraceIdentifier);

        var location = locationFactory(result.Value!);

        return Results.Created(location, response);
    }

    public static IResult ToApiResult(
    this Result result,
    HttpContext httpContext,
    string successMessage = "Request completed successfully.")
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse<object?>.Succeeded(
                data: null,
                message: successMessage,
                traceId: httpContext.TraceIdentifier);

            return Results.Ok(response);
        }

        var error = result.Error!;

        var responseOnFailure = ApiResponse<object?>.Failed(
            error.Message,
            [
                new ApiError(error.Code, error.Message)
            ],
            httpContext.TraceIdentifier);

        var statusCode = GetStatusCode(error.Code);

        return Results.Json(
            responseOnFailure,
            statusCode: statusCode);
    }


    public static IResult ToApiFailure(
             this Result result,
             HttpContext httpContext,
             string errorCode,
             string message,
             int statusCode)
    {
        httpContext.Response.StatusCode = statusCode;

        var payload = new
        {
            success = false,
            error = new
            {
                code = errorCode,
                message,
                details = result.Error?.ToString()
            }
        };

        return Results.Json(payload);
    }



    public static IResult ToApiSuccess(
        this Result result,
        HttpContext httpContext,
        string message)
    {
        var response = ApiResponse<object?>.Succeeded(
            data: null,
            message: message,
            traceId: httpContext.TraceIdentifier);

        return Results.Ok(response);
    }



    public static IResult ToOkResult<T>(
        this Result<T> result)
    {
        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok(result.Value);
    }

    public static IResult ToCreatedResult<T>(
        this Result<T> result,
        string location)
    {
        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Created(location, result.Value);
    }

    public static IResult ToCreatedResult<T>(
        this Result<T> result,
        Func<T, string> locationFactory)
    {
        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error);
        }

        var location = locationFactory(result.Value!);

        return Results.Created(location, result.Value);
    }

    public static IResult ToNoContentResult(
        this Result result)
    {
        if (!result.IsSuccess)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.NoContent();
    }

    private static int GetStatusCode(string errorCode)
    {
        if (errorCode.EndsWith(
                ".Unauthorized",
                StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status401Unauthorized;
        }

        if (errorCode.EndsWith(
                ".Forbidden",
                StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status403Forbidden;
        }

        if (errorCode.EndsWith(
                ".NotFound",
                StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status404NotFound;
        }

        if (errorCode.EndsWith(
                ".Conflict",
                StringComparison.OrdinalIgnoreCase))
        {
            return StatusCodes.Status409Conflict;
        }

        return StatusCodes.Status400BadRequest;
    }
}