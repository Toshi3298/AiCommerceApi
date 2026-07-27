namespace AiCommerceApi.Common.Responses;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public IReadOnlyList<string> Errors { get; init; }
        = Array.Empty<string>();

    public static ApiResponse<T> Ok(
        T? data,
        string message = "İşlem başarılı.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = Array.Empty<string>()
        };
    }

    // Tek hata için kullanılır.
    public static ApiResponse<T> Fail(
        string error,
        string message = "İşlem başarısız.")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = new[] { error }
        };
    }

    // Birden fazla validation hatası için kullanılır.
    public static ApiResponse<T> Fail(
        IEnumerable<string> errors,
        string message = "İşlem başarısız.")
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors
                .Where(error =>
                    !string.IsNullOrWhiteSpace(error))
                .Distinct()
                .ToArray()
        };
    }
}