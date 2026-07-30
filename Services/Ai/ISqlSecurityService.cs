namespace AiCommerceApi.Services.Ai;

public interface ISqlSecurityService
{
    SqlValidationResult Validate(string sql);
}

public sealed class SqlValidationResult
{
    public bool IsValid { get; init; }

    public string? Error { get; init; }

    public static SqlValidationResult Success()
    {
        return new SqlValidationResult
        {
            IsValid = true
        };
    }

    public static SqlValidationResult Failure(string error)
    {
        return new SqlValidationResult
        {
            IsValid = false,
            Error = error
        };
    }
}