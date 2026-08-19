namespace AiCommerceApi.Features.BiaAgent.Planning;

public static class BiaAgentActions
{
    public const string SearchProducts =
        "search_products";

    public const string GetProductDetails =
        "get_product_details";

    public const string SearchThenGetDetails =
        "search_then_get_details";

    public const string GetPreviousProductDetails =
        "get_previous_product_details";

    public const string PrepareAddToCart =
        "prepare_add_to_cart";

    public const string ConfirmPendingAction =
        "confirm_pending_action";

    public const string CancelPendingAction =
        "cancel_pending_action";

    public const string Unsupported =
        "unsupported";
}