namespace AiCommerceApi.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public bool InUse { get; set; }

    public string? Error { get; set; }
}