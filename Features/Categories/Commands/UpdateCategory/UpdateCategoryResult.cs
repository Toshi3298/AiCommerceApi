namespace AiCommerceApi.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public string? Error { get; set; }
}