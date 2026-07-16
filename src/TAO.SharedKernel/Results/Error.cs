namespace TAO.SharedKernel.Results
{
    public sealed record Error(
        string Code,
        string Message
    )
    {
        public static readonly Error None=new Error(string.Empty, string.Empty);

        public static Error Validation(string message) => new Error("Validation", message);

        public static Error NotFound(string message) => new Error("NotFound", message);

        public static Error Unauthorized(string message) => new Error("Unauthorized", message);

        public static Error Forbidden(string message) => new Error("Forbidden", message);

        public static Error Conflict(string message) => new Error("Conflict", message);

        public static Error Failure(string message) => new Error("Failure", message);
    }
}
