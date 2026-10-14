using System.Collections.Concurrent;
using TechStore.Web.Models;

namespace TechStore.Web.Services;

public sealed class InMemoryOrderStore
{
    private readonly ConcurrentDictionary<string, OrderConfirmationViewModel> _orders = new();

    public void Add(OrderConfirmationViewModel order) => _orders[order.OrderNumber] = order;

    public bool TryGet(string orderNumber, out OrderConfirmationViewModel? order) =>
        _orders.TryGetValue(orderNumber, out order);

    public IReadOnlyList<OrderConfirmationViewModel> GetAll() =>
        _orders.Values.OrderByDescending(order => order.CreatedAt).ToArray();
}
