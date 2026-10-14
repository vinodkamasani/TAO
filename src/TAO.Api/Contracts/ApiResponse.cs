namespace TAO.Api.Contracts;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    IReadOnlyCollection<ApiError>? Errors,
    string? TraceId)
{
    public static ApiResponse<T> Succeeded(
        T data,
        string message,
        string? traceId = null)
        => new(
            Success: true,
            Message: message,
            Data: data,
            Errors: null,
            TraceId: traceId);

    public static ApiResponse<T> Failed(
        string message,
        IReadOnlyCollection<ApiError> errors,
        string? traceId = null)
        => new(
            Success: false,
            Message: message,
            Data: default,
            Errors: errors,
            TraceId: traceId);
}