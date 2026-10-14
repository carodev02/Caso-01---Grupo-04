namespace TechStore.Web.Models;

public sealed record AdminOrdersViewModel(
    string AccessKey,
    IReadOnlyList<OrderConfirmationViewModel> Orders);
