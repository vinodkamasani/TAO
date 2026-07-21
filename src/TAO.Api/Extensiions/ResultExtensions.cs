using TAO.SharedKernel.Results;

namespace TAO.Api.Extensions;

public static class ResultExtensions
{
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

        return Results.Created(
            location,
            result.Value);
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
}