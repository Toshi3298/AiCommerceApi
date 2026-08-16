namespace AiCommerceApi.Dtos.Ai;

public sealed class AiProductSearchFilterDto
{
    public string? Search { get; init; }

    public string? Brand { get; init; }

    public string? CategoryName { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public bool? InStock { get; init; }

    public string SortBy { get; init; } = "name";

    public string SortDirection { get; init; } = "asc";

    public int Limit { get; init; } = 50;
}